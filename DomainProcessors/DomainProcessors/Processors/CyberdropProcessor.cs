using AngleSharp.Dom;
using DomainProcessors.Abstractions;
using DomainProcessors.Extensions;
using HttpDocumentHandler.Abstractions;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Processors
{
    internal class CyberdropProcessor : IDomainProcessor
    {
        private const String DomainRegexPattern = @"https?://(.*\.)?cyberdrop.[a-z]{2,3}/";
        private const String TotalFilesQuerySelector = "#totalFilesAmount";
        private const String TitleQuerySelector = "#title";
        private const String TableQuerySelector = "#table";
        private const String ImageQuerySelector = ".image";

        private readonly ILogger<CyberdropProcessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHtmlHandler _htmlHandler;

        public String Domain { get; } = "cyberdrop.me";
        public Regex DomainRegex { get; } = new Regex(DomainRegexPattern);

        public CyberdropProcessor(ILogger<CyberdropProcessor> logger, IHtmlHandler htmlHandler, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _htmlHandler = htmlHandler;
        }

        /// <inheritdoc/>
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            AngleSharp.Html.Dom.IHtmlDocument? document = await _htmlHandler.GetHtmlDocumentAsync(galeryUrl, _httpClientFactory.CreateClient(), cancellationToken);
            return Int32.Parse(document.QuerySelectorOrThrow(TotalFilesQuerySelector).TextContent);
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRequest = new WorkRequest();
                foreach (var url in urls)
                {
                    if (!workRequest.TryAdd(await ProcessUrlAsync(url, cancellationToken)))
                    {
                        _logger.LogWarning($"Duplicate URL [{url}] found. Ignoring...");
                    }
                }
                return workRequest;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new WorkRequest();
                result.Add(await ProcessUrlAsync(galeryUrl, cancellationToken));
                return result;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        private async Task<WorkSet> ProcessUrlAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            AngleSharp.Html.Dom.IHtmlDocument? document = await _htmlHandler.GetHtmlDocumentAsync(galeryUrl, _httpClientFactory.CreateClient(), cancellationToken);
            var workset = new WorkSet(galeryUrl, document.QuerySelector(TitleQuerySelector)?.GetAttribute("title") ?? String.Empty, GetDefaultHttpClient());

            IElement tableElement = document.QuerySelectorOrThrow(TableQuerySelector);
            foreach (IElement element in tableElement.QuerySelectorAll(ImageQuerySelector))
            {
                var href = element.GetAttribute("href");
                if (!String.IsNullOrEmpty(href))
                {
                    workset.Add(new UnitOfWork(href));
                }
            }
            return workset;
        }

        private HttpClient GetDefaultHttpClient()
        {
            HttpClient? httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = Timeout.InfiniteTimeSpan;
            return httpClient;
        }

        private void HandleException(Exception exception)
        {
            _logger.LogError(exception.Message);
            _logger.LogException(exception);
        }
    }
}
