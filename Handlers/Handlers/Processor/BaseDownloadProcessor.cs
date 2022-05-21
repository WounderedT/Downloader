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
    internal class BaseDownloadProcessor : IDownloadProcessor
    {
        private const Int32 DownloadFragments = 100;
        private const String TemporarilyMovedReasonPhrase = "Moved Temporarily";
        private readonly DownloadHandlerConfig _options;
        private readonly ILogger<DefaultDownloadHandler> _logger;
        private readonly IStorageProvider _storageProvider;

        public StrategyEnum DownloadStrategy { get; } = StrategyEnum.Default;

        public BaseDownloadProcessor(IOptions<DownloadHandlerConfig> options, ILogger<DefaultDownloadHandler> logger, IStorageProvider storageProvider)
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
                var tmpFilename = unitOfWork.Filename + _options.DownloadExtension;
                try
                {
                    var result = await DownloadInternal(unitOfWork.Url, client, path, tmpFilename, cancellationToken);
                    return result ? CommitDownload(path, tmpFilename, unitOfWork) : false;
                }
                catch (Exception ex)
                {
                    HandleIOException(ex, path, tmpFilename);
                    return false;
                }
            }
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

        private Boolean CommitDownload(String path, String tmpFilename, UnitOfWork unitOfWork)
        {
            _storageProvider.RenameFile(path, tmpFilename, unitOfWork.Filename);
            unitOfWork.IsDownloaded = true;
            return true;
        }

        private async Task<Boolean> DownloadInternal(Uri url, HttpClient client, String path, String filename, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (HttpResponseMessage response = await GetResponseMessage(url, client))
            {
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to download [{url}] - response code [{response.StatusCode}] does not indicate success");
                    return false;
                }
                if (response.Content.Headers.ContentLength == null)
                {
                    _logger.LogError($"Expected ContentLength of [{url}] cannot be null");
                    return false;
                }

                try
                {
                    using (Stream fileStream = _storageProvider.OpenStreamForWrite(path, filename))
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
                        await fileStream.WriteAsync(content, cancellationToken);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    HandleIOException(ex, path, filename);
                    return false;
                }
            }
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
