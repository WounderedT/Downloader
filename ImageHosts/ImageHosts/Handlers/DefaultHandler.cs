using ImageHosts.Abstractions;
using Microsoft.Extensions.Logging;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace ImageHosts.Handlers
{
    internal class DefaultHanlder : IImageHostHandler
    {
        private readonly ILogger<DefaultHanlder> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public DefaultHanlder(ILogger<DefaultHanlder> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        #region Implementation of IImageHostHandler

        /// <inheritdoc />
        public String ImageHost { get; } = Constants.DefaultHandler;

        /// <inheritdoc />
        public async Task<UnitOfWork> HandleUrlAsync(String url)
        {
            return await HandleUrlAsync(url, _httpClientFactory.CreateClient());
        }

        /// <inheritdoc />
        public async Task<UnitOfWork> HandleUrlAsync(String url, HttpClient httpClient)
        {
            if(url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            if(httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            //Verifying if provided URL is valid by perfroming GET request and checking response code. Not performance friendly.
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return new UnitOfWork(url);
        }

        /// <inheritdoc />
        public async Task<WorkRequest> HandleUrlsAsync(IEnumerable<String> urls)
        {
            return await HandleUrlsAsync(urls, _httpClientFactory.CreateClient());
        }

        /// <inheritdoc />
        public async Task<WorkRequest> HandleUrlsAsync(IEnumerable<String> urls, HttpClient httpClient)
        {
            if (urls == null || !urls.Any())
            {
                throw new ArgumentNullException(nameof(urls));
            }
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            var request = new WorkRequest();
            var workset = new WorkSet(urls.FirstOrDefault());
            foreach (var url in urls)
            {
                workset.Add(await HandleUrlAsync(url, httpClient));
            }
            request.Add(workset);
            return request;
        }

        #endregion
    }
}
