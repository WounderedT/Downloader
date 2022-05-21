using Handlers.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProgressHandler.Abstractions;
using Shared.Extensions;
using Shared.Models.Work;

namespace Handlers.ConcurrentProcessing
{
    internal class ConcurrentExecutionProcessor : IConcurrentExecutionProcessor
    {
        private readonly DownloadHandlerConfig _options;
        private readonly ILogger<ConcurrentExecutionProcessor> _logger;
        private readonly IProgressHandler _progressHandler;

        public ConcurrentExecutionProcessor(IOptions<DownloadHandlerConfig> options, ILogger<ConcurrentExecutionProcessor> logger, IProgressHandler progressHandler)
        {
            _options = options.Value;
            _logger = logger;
            _progressHandler = progressHandler;
        }

        public async Task<Boolean> ProcessInParallelAsync(IEnumerable<UnitOfWork> unitsOfWork, Func<UnitOfWork, Task<Boolean>> processingFunc)
        {
            return await ProcessInParallelAsync(unitsOfWork, processingFunc, Environment.ProcessorCount);
        }

        public async Task<Boolean> ProcessInParallelAsync(IEnumerable<UnitOfWork> unitsOfWork, Func<UnitOfWork, Task<Boolean>> processingFunc, Int32 threads)
        {
            var processingList = new List<Task<Boolean>>();
            IEnumerator<UnitOfWork> enumerator = unitsOfWork.GetEnumerator();
            while (processingList.Count <= threads && enumerator.MoveNext())
            {
                _logger.LogDebug($"Starting [{enumerator.Current.Url.AbsoluteUri}] concurrent processing.");
                processingList.Add(processingFunc(enumerator.Current));
            }

            var result = true;
            while (processingList.Count > 0)
            {
                var complete = await Task.WhenAny(processingList);
                if (complete.Exception != null)
                {
                    _logger.LogError($"An exception was caught while processing: [{complete.Exception.Message}].");
                    _logger.LogException(complete.Exception);
                    if (_options.StopOnError)
                    {
                        enumerator.Dispose();
                        await Task.WhenAll(processingList);
                        return false;
                    }
                }
                else if (!complete.Result && _options.StopOnError)
                {
                    enumerator.Dispose();
                    await Task.WhenAll(processingList);
                    return false;
                }

                result &= complete.Exception == null ? complete.Result : false;
                processingList.Remove(complete);
                _progressHandler.Update();

                if (!enumerator.MoveNext())
                {
                    continue;
                }

                _logger.LogDebug($"Starting [{enumerator.Current.Url.AbsoluteUri}] concurrent processing.");
                processingList.Add(processingFunc(enumerator.Current));
            }

            return result;
        }
    }
}
