using Handlers.Configuration;
using Handlers.Models;
using Infra.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Models.Work;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace Handlers.Processor
{
    internal class FragmentedDownloadProcessor : IDownloadProcessor
    {
        private const Int32 DownloadFragments = 100;
        private const String TemporarilyMovedReasonPhrase = "Moved Temporarily";
        private readonly DownloadHandlerConfig _options;
        private readonly ILogger<DefaultDownloadHandler> _logger;
        private readonly IStorageProvider _storageProvider;

        public StrategyEnum DownloadStrategy { get; } = StrategyEnum.Fragmented;

        public FragmentedDownloadProcessor(IOptions<DownloadHandlerConfig> options, ILogger<DefaultDownloadHandler> logger, IStorageProvider storageProvider)
        {
            _logger = logger;
            _storageProvider = storageProvider;
            _options = options.Value;
            _options.DownloadExtension = _options.DownloadExtension.StartsWith('.') ? _options.DownloadExtension : "." + _options.DownloadExtension;
        }

        public async Task<Boolean> DownloadAsync(UnitOfWork unitOfWork, String path, HttpClient client, CancellationToken cancellationToken)
        {
            using (_logger.Scope($"{nameof(UnitOfWork)}.Id", unitOfWork.Id))
            using (HttpResponseMessage response = await GetResponseMessage(unitOfWork.Url, client))
            {
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to download [{unitOfWork.Url}] - response code [{response.StatusCode}] does not indicate success");
                    return unitOfWork.IsDownloaded;
                }
                if (response.Content.Headers.ContentLength == null)
                {
                    _logger.LogError($"Expected ContentLength of [{unitOfWork.Url}] cannot be null");
                    return unitOfWork.IsDownloaded;
                }
                var tmpFilename = unitOfWork.Filename + _options.DownloadExtension;
                var initialFragment = GetInitialFragment(tmpFilename, response.Content.Headers.ContentLength.Value);
                if (initialFragment.Id > DownloadFragments)
                {
                    return CommitDownload(path, tmpFilename, unitOfWork);
                }

                try
                {
                    using(Stream tmpFileStream = _storageProvider.OpenStreamForWrite(path, tmpFilename))
                    {
                        await DownloadInFragments(unitOfWork.Url, initialFragment, client, tmpFileStream, cancellationToken);
                    }
                    return CommitDownload(path, tmpFilename, unitOfWork);
                }
                catch (Exception ex)
                {
                    HandleIOException(ex, path, tmpFilename);
                }
            }
            return unitOfWork.IsDownloaded;
        }

        private async Task<HttpResponseMessage> GetResponseMessage(Uri url, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage? response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            while (response.ReasonPhrase == TemporarilyMovedReasonPhrase)
            {
                response = await client.GetAsync(response.Headers.Location, HttpCompletionOption.ResponseHeadersRead);
            }
            return response;
        }

        private Fragment GetInitialFragment(String path, Int64 expectedLength)
        {
            var fragmentSize = expectedLength / DownloadFragments;
            if (_storageProvider.FileExists(path))
            {
                var fileLength = _storageProvider.GetFileLength(path);
                if (expectedLength == fileLength)
                {
                    //File is already downloaded but not renamed. Returning DownloadFragments + 1 fragment.
                    return new Fragment(DownloadFragments + 1, fragmentSize, expectedLength);
                }
                return new Fragment((Int32)(fileLength / fragmentSize), fragmentSize, expectedLength);
            }
            return new Fragment(0, fragmentSize, expectedLength);
        }

        private Boolean CommitDownload(String path, String tmpFilename, UnitOfWork unitOfWork)
        {
            _storageProvider.RenameFile(path, tmpFilename, unitOfWork.Filename);
            unitOfWork.IsDownloaded = true;
            return true;
        }

        private async Task DownloadInFragments(Uri url, Fragment fragment, HttpClient client, Stream tmpFileStream, CancellationToken cancellationToken)
        {
            for (Int32 fragmentId = fragment.Id; fragmentId <= DownloadFragments; fragmentId++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (HttpRequestMessage request = BuildFragmentRequestMessage(url, fragmentId, fragment))
                using (HttpResponseMessage rangedResponse = await client.SendAsync(request, cancellationToken))
                {
                    var contentFragmentBytes = await rangedResponse.Content.ReadAsByteArrayAsync();
                    tmpFileStream.Seek(request.Headers.Range.Ranges.First().From.GetValueOrDefault(), SeekOrigin.Begin);
                    await tmpFileStream.WriteAsync(contentFragmentBytes, cancellationToken);
                }
            }
        }

        private HttpRequestMessage BuildFragmentRequestMessage(Uri url, Int32 fragmentId, Fragment fragment)
        {
            Int64 fragmentStartPosition = fragmentId * fragment.FragmentSize;
            Int64 fragmentEndPosition = fragmentId < DownloadFragments ? (fragmentId + 1) * fragment.FragmentSize : fragment.ExpectedLengthInBytes;

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = url;
            request.Headers.Range = new RangeHeaderValue(fragmentStartPosition, fragmentEndPosition);
            
            return request;
        } 

        private void HandleIOException(Exception exception, String path, String filename)
        {
            _logger.LogWarning($"An exception was caught while handling download request. {exception.GetType().Name}: {exception.Message}");
            if (_storageProvider.FileExists(Path.Combine(path, filename)))
            {
                try
                {
                    _storageProvider.DeleteFile(Path.Combine(path, filename));
                }
                catch(Exception ex)
                {
                    _logger.LogWarning($"An exception was caught while removing temporary dowload file: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
}
