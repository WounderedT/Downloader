using Handlers.Abstractions;
using Handlers.Configuration;
using Handlers.Models;
using Handlers.Processor;
using Handlers.Strategy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.DownloadModel
{
    internal class DownloadProcessorFactory: IDownloadProcessorFactory
    {
        private readonly ILogger<DownloadProcessorFactory> _logger;
        private readonly IStrategyFactory _strategyFactory;
        private readonly Dictionary<StrategyEnum, IDownloadProcessor> _modelsByStrategy;

        public DownloadProcessorFactory(ILogger<DownloadProcessorFactory> logger, IEnumerable<IDownloadProcessor> downloadHandler, IStrategyFactory strategyFactory)
        {
            _logger = logger;
            _modelsByStrategy = downloadHandler.ToDictionary(handler => handler.DownloadStrategy, handler => handler);
            _strategyFactory = strategyFactory;
        }

        /// <inheritdoc/>
        public IDownloadProcessor GetDownloadProcessor(String domain)
        {
            StrategyEnum strategy = _strategyFactory.GetStrategy(domain);
            if (!_modelsByStrategy.TryGetValue(strategy, out var downloadModel))
            {
                _logger.LogError($"[{strategy}] has no defined imlpementation.");
                throw new ArgumentException($"[{strategy}] has no defined imlpementation.");
            }
            return downloadModel;
        }

        /// <inheritdoc/>
        public IDownloadProcessor GetDownloadProcessor(HttpContentHeaders headers)
        {
            StrategyEnum strategy = _strategyFactory.GetStrategy(headers);
            if(!_modelsByStrategy.TryGetValue(strategy, out var downloadModel))
            {
                _logger.LogError($"[{strategy}] has no defined imlpementation.");
                throw new ArgumentException($"[{strategy}] has no defined imlpementation.");
            }
            return downloadModel;
        }
    }
}
