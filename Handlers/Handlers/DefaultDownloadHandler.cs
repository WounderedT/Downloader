using Handlers.Abstractions;
using Handlers.ConcurrentProcessing;
using Handlers.Configuration;
using Handlers.Configuration.Retry;
using Handlers.DownloadModel;
using Handlers.Processor;
using Infra.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Models.Work;

namespace Handlers
{
    internal class DefaultDownloadHandler : IDownloadHandler
    {
        private readonly ILogger<DefaultDownloadHandler> _logger;
        private readonly IStorageProvider _storageProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DownloadHandlerConfig _options;
        private readonly IConcurrentExecutionProcessor _concurrentProcessor;
        private readonly IDownloadProcessorFactory _downloadProcessorFactory;

        /// <inheritdoc/>
        public String DownloaderType { get; } = Constants.DefaultDownloaderType;

        public DefaultDownloadHandler(IOptions<DownloadHandlerConfig> options, ILogger<DefaultDownloadHandler> logger, 
            IHttpClientFactory clientFactory, IStorageProvider storageProvider, IConcurrentExecutionProcessor concurrentProcessor, 
            IDownloadProcessorFactory downloadProcessorFactory)
        {
            _options = options.Value;
            _logger = logger;
            _storageProvider = storageProvider;
            _httpClientFactory = clientFactory;
            _concurrentProcessor = concurrentProcessor;
            _downloadProcessorFactory = downloadProcessorFactory;
        }

        /// <inheritdoc/>
        public async Task<Boolean> DownloadAsync(WorkSet workSet, String path, Boolean skipMissing = false, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                _logger.LogWarning("Downloader disabled.");
                return false;
            }
            return await DownloadAsync(workSet, path, _httpClientFactory.CreateClient(), skipMissing, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Boolean> DownloadAsync(WorkSet workSet, String path, HttpClient client, Boolean skipMissing = false, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                _logger.LogWarning("Downloader disabled.");
                return false;
            }
            if (workSet == null || !workSet.Any())
            {
                throw new ArgumentNullException(nameof(workSet));
            }
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            using var scopeId = _logger.Scope($"{nameof(WorkSet)}.Id", workSet.Id);
            if (!String.IsNullOrWhiteSpace(workSet.FolderName))
            {
                path = Path.Combine(path, workSet.FolderName);
            }
            _storageProvider.CreatePath(path);
            IDownloadProcessor processor = _downloadProcessorFactory.GetDownloadProcessor(workSet.Domain);

            RetryPolicyEntryConfig? retryPolicy = _options.RetryPolicyByDomain.GetPolicyOrDefault(workSet.Domain);
            var threads = retryPolicy.MaxParallelProcessingCount;
            IEnumerable<UnitOfWork>? unitsOfWork = workSet.Where(u => !u.IsDownloaded);
            (var status, unitsOfWork) = await DownloadInternalAsync(unitsOfWork, path, client, processor, threads, cancellationToken);
            if (status || !_options.RetryOnFailure)
            {
                return status;
            }

            for (var retryAttempt = 0; retryAttempt < retryPolicy.Retries; retryAttempt++)
            {
                _logger.LogInformation($"Retrying failed download processing. Attemp [{retryAttempt + 1}] out of [{retryPolicy.Retries}]");
                (status, unitsOfWork) = await DownloadInternalAsync(unitsOfWork, path, client, processor, retryPolicy.ScaleThreadsCount(threads), cancellationToken);
                if (status)
                {
                    return status;
                }
            }

            _logger.LogInformation($"Download failed. Failed to download [{unitsOfWork.Count()}] out of [{workSet.Count}] items.");
            foreach (UnitOfWork? failedUnitOfWork in unitsOfWork)
            {
                _logger.LogDebug($"{failedUnitOfWork.Url.AbsoluteUri}");
            }

            return status;
        }

        private async Task<(Boolean, IEnumerable<UnitOfWork>?)> DownloadInternalAsync(IEnumerable<UnitOfWork> unitsOfWork, String path, HttpClient client, IDownloadProcessor downloadProcessor, Int32 threads, CancellationToken cancellationToken)
        {
            var totalUnitsOfWork = unitsOfWork.Count();
            _logger.LogInformation($"Processing [{totalUnitsOfWork}] download request{(totalUnitsOfWork != 1 ? "s" : "")} to [{path}] dir.");
            var status = await _concurrentProcessor.ProcessInParallelAsync(unitsOfWork, url => downloadProcessor.DownloadAsync(url, path, client, cancellationToken), threads);
            if (status)
            {
                _logger.LogInformation($"Processing complete. [{totalUnitsOfWork}] download request{(totalUnitsOfWork != 1 ? "s were" : " was")} handled.");
                return (true, null);
            }
            else
            {
                var downloadedUnitsOfWork = unitsOfWork.Where(u => u.IsDownloaded).Count();
                _logger.LogError($"Processing failed. [{downloadedUnitsOfWork}] out of [{totalUnitsOfWork}] download request{(totalUnitsOfWork != 1 ? "s were" : " was")} handled.");
                return (false, unitsOfWork.Where(u => !u.IsDownloaded));
            }
        }
    }
}
