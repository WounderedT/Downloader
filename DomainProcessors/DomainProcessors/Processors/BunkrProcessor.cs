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
    internal class BunkrProcessor : IDomainProcessor
    {
        private const String DomainRegexPattern = @"https?://bunkr.[a-z]{2,3}/";
        private const String TotalFilesPattern = @"([0-9]{1,}) file.*";
        private const String TotalFilesQuerySelector = "#count";
        private const String TitleQuerySelector = "#title";
        private const String TableQuerySelector = "#table";
        private const String ImageQuerySelector = ".image";
        private const Int32 TotalFilesGroupId = 1;

        private readonly ILogger<BunkrProcessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHtmlHandler _htmlHandler;
        
        private Regex _totalFilesPattern = new Regex(TotalFilesPattern);

        public String Domain { get; } = "bunkr.is";
        public Regex DomainRegex { get; } = new Regex(DomainRegexPattern);

        public BunkrProcessor(ILogger<BunkrProcessor> logger, IHtmlHandler htmlHandler, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _htmlHandler = htmlHandler;
        }

        /// <inheritdoc/>
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            AngleSharp.Html.Dom.IHtmlDocument? document = await _htmlHandler.GetHtmlDocumentAsync(galeryUrl, _httpClientFactory.CreateClient(), cancellationToken);
            var totalFilesStr = document.QuerySelector(TotalFilesQuerySelector)?.TextContent;
            if (String.IsNullOrWhiteSpace(totalFilesStr))
            {
                _logger.LogWarning($"Failed to get element count on [{galeryUrl}] using [{TotalFilesQuerySelector}] query selector");
                return -1;
            }
            var totalFilesRegexMatch = _totalFilesPattern.Match(totalFilesStr);
            if(!totalFilesRegexMatch.Success)
            {
                _logger.LogWarning($"Failed to parse [{totalFilesStr}] using [{TotalFilesPattern}] regex");
                return -1;
            }
            return Int32.Parse(totalFilesRegexMatch.Groups[TotalFilesGroupId].Value);
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
            var workset = new WorkSet(galeryUrl, document.QuerySelector(TitleQuerySelector)?.TextContent.Trim() ?? String.Empty, GetDefaultHttpClient());

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
