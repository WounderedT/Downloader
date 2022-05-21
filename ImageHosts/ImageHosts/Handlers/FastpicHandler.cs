using ImageHosts.Abstractions;
using ImageHosts.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace ImageHosts.Handlers
{
    internal class FastpicHandler : IImageHostHandler
    {
        private const String _urlRegexPattern = @"https?://fastpic\.org/view/([0-9]{1,})/([0-9]{4})/([0-9]{4})/((.*)(.{2})\.[a-zA-Z]{2,4})\.html";
        private const Int32 _iGroupIndex = 1;
        private const Int32 _yearGroupIndex = 2;
        private const Int32 _monthDateGroupIndex = 3;
        private const Int32 _filenameGroupIndex = 4;
        private const Int32 _indexGroupIndex = 6;

        private readonly ILogger<FastpicHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Regex _urlRegex = new Regex(_urlRegexPattern);

        public FastpicHandler(ILogger<FastpicHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        #region Implementation of IImageHostHandler

        /// <inheritdoc />
        public String ImageHost { get; } = "fastpic.org";

        /// <inheritdoc />
        public Task<UnitOfWork> HandleUrlAsync(String url)
        {
            return HandleUrlAsync(url, _httpClientFactory.CreateClient());
        }

        /// <inheritdoc />
        public Task<UnitOfWork> HandleUrlAsync(String url, HttpClient httpClient)
        {
            var match = _urlRegex.Match(url);
            if (!match.Success)
            {
                _logger.LogError($"Url processing failed [{url}]: URL regex returned no match!");
                throw new ImageHostHandlerException($"Url processing failed [{url}]: URL regex returned no match!");
            }
            return Task.FromResult(new UnitOfWork($"https://i{match.Groups[_iGroupIndex].Value}.fastpic.org/big/{match.Groups[_yearGroupIndex].Value}/{match.Groups[_monthDateGroupIndex].Value}/{match.Groups[_indexGroupIndex].Value}/{match.Groups[_filenameGroupIndex].Value}"));
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
