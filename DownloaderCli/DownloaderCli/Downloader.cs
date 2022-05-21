
using DomainProcessors.Abstractions;
using DownloaderCli.Configuration;
using DownloaderCli.Interfaces;
using DownloaderCli.Resume;
using Handlers.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationProvider.Abstractions;
using ProgressHandler.Abstractions;
using Shared.Extensions;
using Shared.Models.Result;
using Shared.Models.Work;
using System;
using System.IO;

namespace DownloaderCli
{
    internal class Downloader : IDownloader
    {
        private readonly ILogger<Downloader> _logger;
        private readonly DownloaderCliConfig _options;
        private readonly IDomainProcessorFactory _domainProcessorFactory;
        private readonly IDownloadHandlerFactory _downloadHandlerFactory;
        private readonly IProgressHandler _progressHandler;
        private readonly INotificationProvider _notificationProvider;
        private readonly IResumeHandler _resumeHandler;

        public Downloader(IOptions<DownloaderCliConfig> options, ILogger<Downloader> logger, IDomainProcessorFactory domainProcessorFactory,
            IDownloadHandlerFactory downloadHandlerFactory, IProgressHandler progressHandler, INotificationProvider notificationProvider, IResumeHandler resumeHandler)
        {
            _logger = logger;
            _options = options.Value;
            _domainProcessorFactory = domainProcessorFactory;
            _downloadHandlerFactory = downloadHandlerFactory;
            _progressHandler = progressHandler;
            _notificationProvider = notificationProvider;
            _resumeHandler = resumeHandler;
        }

        ///<inheritdoc/>
        public async Task<DownloadResult> DownloadAsync(IEnumerable<String> urls, String path)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }
            if (urls.Count() == 0)
            {
                throw new ArgumentException(nameof(urls));
            }
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            DownloadResult result = new DownloadResult(path);
            try
            {
                await DownloadInternalAsync(urls, path, result);
            }
            catch (Exception ex)
            {
                HandleException(ex, result, $"An exception was caught while processing [{urls.Count()}] urls.");
            }
            return result;
        }

        ///<inheritdoc/>
        public async Task<DownloadResult> DownloadAsync(String url, String path)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            DownloadResult result = new DownloadResult(path);
            try
            {
                await DownloadInternalAsync(url, path, result);
            }
            catch (Exception ex)
            {
                HandleException(ex, result, $"An exception was caught while processing [{url}]");
            }
            return result;
        }

        ///<inheritdoc/>
        public async Task<DownloadResult> RetryAsync(DownloadResult originalResult, String path, Boolean force = false)
        {
            if(originalResult == null)
            {
                throw new ArgumentNullException(nameof(originalResult));    
            }
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (originalResult.Urls.Count == 0)
            {
                throw new ArgumentException($"{nameof(originalResult)} must have at least one URL.");
            }
            if (originalResult.IsSuccess && !force)
            {
                _logger.LogInformation($"Provided {nameof(DownloadResult)} was successfully downloaded. Use {nameof(force)} to restart completed download.");
                return originalResult;
            }

            DownloadResult result;
            WorkRequest workRequest;
            if (originalResult.OriginalWorkRequest == null || originalResult.OriginalWorkRequest.Count == 0)
            {
                result = new DownloadResult(path);
                try
                {
                    workRequest = await BuildWorkRequestAsync(originalResult.Urls);
                    if (workRequest.Count == 0)
                    {
                        _logger.LogError($"Failed to process URLs.");
                        _notificationProvider.PopErrorToast($"Failed to process URLs.");
                        result.IsSuccess = false;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex, result, $"An exception was caught while processing [{originalResult.Urls.Count()}] urls.");
                    return result;
                }
                result.OriginalWorkRequest = workRequest;
            }
            else
            {
                workRequest = new WorkRequest();
                workRequest.AddRange(originalResult.OriginalWorkRequest.Where(work => !work.IsSuccess));
                result = new DownloadResult(workRequest, path);
                result.OriginalWorkRequest = workRequest;
            }
            try
            {
                await ProcessWorkRequestAsync(workRequest, path, result);
            }
            catch (Exception ex)
            {
                HandleException(ex, result, $"An exception was caught while processing [{result.Urls.Count()}] urls.");
            }
            return result;
        }

        private async Task DownloadInternalAsync(IEnumerable<String> urls, String path, DownloadResult result)
        {
            _logger.LogInformation($"Starting download of [{urls.Count()}] URL{(urls.Count() > 1 ? "s" : "")} to [{path}] directory.");
            WorkRequest workRequest = await BuildWorkRequestAsync(urls);
            if (workRequest.Count == 0)
            {
                _logger.LogError($"Failed to process URLs.");
                _notificationProvider.PopErrorToast($"Failed to process URLs.");
                result.IsSuccess = false;
                return;
            }
            result.OriginalWorkRequest = workRequest;
            await ProcessWorkRequestAsync(workRequest, path, result);
        }

        private async Task DownloadInternalAsync(String url, String path, DownloadResult result)
        {
            _logger.LogInformation($"Downloading [{url.DecodeUrlString()}] to [{path}] directory.");
            WorkRequest workRequest = await BuildWorkRequestAsync(url);
            if (workRequest == null)
            {
                _logger.LogError($"Failed to process items on [{url}].");
                _notificationProvider.PopErrorToast($"Failed to process items on [{url}].");
                result.IsSuccess = false;
                return;
            }
            result.OriginalWorkRequest = workRequest;
            await ProcessWorkRequestAsync(workRequest, path, result);
        }

        private async Task ProcessWorkRequestAsync(WorkRequest request, String path, DownloadResult result)
        {
            _progressHandler.Start($"Downloading", (UInt32)request.Count);
            IDownloadHandler? downloadHandler = _downloadHandlerFactory.GetDefaultHandler();
            var downloadStatus = true;
            var worksetCount = 1;
            foreach (WorkSet workSet in request)
            {
                using (_logger.Scope("Id", workSet.Id))
                _logger.LogInformation($"Processing {nameof(WorkSet)} [{worksetCount++}] out of [{request.Count}].");
                _resumeHandler.HandlerResume(workSet, path);
                if (workSet.IsSuccess)
                {
                    _logger.LogInformation($"{nameof(WorkSet)} has been downloaded before. Nothing to do.");
                    result.AddResult(workSet);
                    _progressHandler.Update();
                    continue;
                }

                var isSuccess = workSet.HttpClient != null ? await downloadHandler.DownloadAsync(workSet, path, workSet.HttpClient, true) : await downloadHandler.DownloadAsync(workSet, path, true);
                downloadStatus &= isSuccess;
                result.AddResult(workSet);

                if (isSuccess)
                {
                    _logger.LogInformation($"{nameof(WorkSet)} download complete.");
                }
                else
                {
                    _logger.LogError($"{nameof(WorkSet)} download failed.");
                }
                _progressHandler.Update();
            }
            _progressHandler.Stop();
            if (downloadStatus)
            {
                var message = $"Download complete. Content from [{request.Count}] {nameof(WorkSet)}{(request.Count > 1 ? "s" : "")} was downloaded.";
                _logger.LogInformation(message);
                _notificationProvider.PopInfoToast(message, path);
            }
            else
            {
                var message = $"Download failed. Content from [{result.SuccessCount}] out of [{result.TotalCount}] {nameof(WorkSet)}{(result.TotalCount > 1 ? "s" : "")} was downloaded.";
                _logger.LogInformation(message);
                _notificationProvider.PopErrorToast(message);
            }
        }

        private async Task<WorkRequest> BuildWorkRequestAsync(IEnumerable<String> urls)
        {
            _logger.LogInformation($"Building {nameof(WorkRequest)} for [{urls.Count()}] URL{(urls.Count() > 1 ? "s" : "")}.");
            var dictionary = new Dictionary<IDomainProcessor, List<String>>();
            var workRequest = new WorkRequest();
            foreach (var url in urls)
            {
                var processor = _domainProcessorFactory.GetDomainProcessor(url);
                if (dictionary.TryGetValue(processor, out List<String>? processorUrls))
                {
                    processorUrls.Add(url);
                }
                else
                {
                    dictionary.Add(processor, new List<String>() { url });
                }
            }

            _logger.LogInformation($"Processing [{urls.Count()}] URL{(urls.Count() > 1 ? "s" : "")} using {dictionary.Count} {nameof(DomainProcessors)}");
            _progressHandler.Start($"Building {nameof(WorkRequest)}", (UInt32)dictionary.Keys.Count);
            foreach (IDomainProcessor? processor in dictionary.Keys)
            {
                workRequest.Merge(await processor.ProcessAsync(dictionary[processor]));
                _progressHandler.Update();
            }
            _progressHandler.Stop();
            return workRequest;
        }

        private async Task<WorkRequest> BuildWorkRequestAsync(String url)
        {
            _logger.LogInformation($"Building {nameof(WorkRequest)} for the URL.");
            var processor = _domainProcessorFactory.GetDomainProcessor(url);
            var imageCount = await processor.GetElementsCountAsync(url);
            if (imageCount > 0)
            {
                _progressHandler.Start($"Building {nameof(WorkRequest)}", (UInt32)imageCount);
            }
            WorkRequest work = await processor.ProcessAsync(url);
            if (imageCount > 0)
            {
                _progressHandler.Stop();
            }
            return work;
        }

        private void HandleException(Exception exception, DownloadResult result, String message)
        {
            _logger.LogError(message);
            _logger.LogException(exception);
            _notificationProvider.PopErrorToast(message);
            result.AddException(exception);
        }
    }
}
