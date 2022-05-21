using DomainProcessors.Abstractions;
using DomainProcessors.Abstractions.Exceptions;
using ImageHosts.Abstractions;
using Microsoft.Extensions.Logging;
using Shared.Models.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DomainProcessors.Processors
{
    internal class DefaultProcessor : IDomainProcessor
    {
        private readonly ILogger<ReactorProcessor> _logger;
        private readonly IImageHostHandlerFactory _imageHostHandlerFactory;

        public String Domain { get; } = Constants.DefaultProcessor;
        public Regex? DomainRegex => null;

        public DefaultProcessor(ILogger<ReactorProcessor> logger, IImageHostHandlerFactory imageHostHandlerFactory)
        {
            _logger = logger;
            _imageHostHandlerFactory = imageHostHandlerFactory;
        }

        /// <inheritdoc/>
        public Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRequest = new WorkRequest();
                var workset = new WorkSet(galeryUrl);
                workset.Add(await _imageHostHandlerFactory.GetHandler(galeryUrl).HandleUrlAsync(galeryUrl));
                workRequest.Add(workset);
                return workRequest;
            }
            catch (Exception exception)
            {
                throw new InternalUrlProcessingException(galeryUrl, exception);
            }
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            var workRequest = new WorkRequest();
            var handlers = new Dictionary<IImageHostHandler, List<String>>();
            
            try
            {
                foreach (var url in urls)
                {
                    var handler = _imageHostHandlerFactory.GetHandler(url);
                    if (handlers.TryGetValue(handler, out List<String>? urlsToHandle))
                    {
                        urlsToHandle.Add(url);
                    }
                    else
                    {
                        handlers.Add(handler, new List<String>() { url });
                    }
                }

                _logger.LogInformation($"Processing [{urls.Count()}] URL{(urls.Count() > 1 ? "s" : "")} using {handlers.Count} {nameof(IImageHostHandler)}");
                foreach (var pair in handlers)
                {
                    workRequest.Merge(await pair.Key.HandleUrlsAsync(pair.Value));
                }
            }
            catch (Exception exception)
            {
                throw new InternalUrlProcessingException(exception);
            }

            return workRequest;
        }
    }
}
