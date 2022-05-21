using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HttpDocumentHandler.Abstractions;
using HttpDocumentHandler.Abstractions.Exceptions;
using HttpDocumentHandler.Cache;
using Microsoft.Extensions.Logging;

namespace HttpDocumentHandler
{
    internal class HtmlHandler : IHtmlHandler
    {
        private readonly ILogger<HtmlHandler> _logger;
        private readonly IHtmlDocumentCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public HtmlHandler(ILogger<HtmlHandler> logger, IHttpClientFactory httpClientFactory, IHtmlDocumentCache htmlDocumentCache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cache = htmlDocumentCache;
        }

        ///<inheritdoc/>
        public Task<IHtmlDocument> GetHtmlDocumentAsync(String url, CancellationToken cancellationToken = default)
        {
            return GetHtmlDocumentAsync(url, _httpClientFactory.CreateClient(), cancellationToken);
        }

        ///<inheritdoc/>
        public async Task<IHtmlDocument> GetHtmlDocumentAsync(String url, HttpClient httpClient, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            if (_cache.TryGetDocument(url, out IHtmlDocument? document))
            {
                if (document != null)
                {
                    return document;
                }
                _logger.LogWarning($"NULL object found in cache for {url}.");
            }

            try
            {
                HttpResponseMessage? response = await GetAsync(url, httpClient, cancellationToken);
                document = await new HtmlParser().ParseDocumentAsync(response.Content.ReadAsStream(), cancellationToken);
                _cache.Add(url, document);
                return document;
            }
            catch (Exception ex)
            {
                throw new HtmlDocumentProcessingException($"Exception was caught while getting html document for {url}", ex);
            }
        }

        private async Task<HttpResponseMessage> GetAsync(String url, HttpClient client, CancellationToken cancellationToken)
        {
            HttpResponseMessage? response = await client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get respose from [{url}]: {response.StatusCode} - {response.ReasonPhrase}");
                response.EnsureSuccessStatusCode();
            }
            return response;
        }
    }
}
