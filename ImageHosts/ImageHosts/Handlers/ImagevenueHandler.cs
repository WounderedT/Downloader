using HttpDocumentHandler.Abstractions;
using ImageHosts.Abstractions;
using ImageHosts.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using Shared.Models.Work;

namespace ImageHosts.Handlers
{
    internal class ImagevenueHandler : IImageHostHandler
    {
        private String _SelectorMain = "main";
        private String _SelectorImage = "img";
        private String _UrlAttribute = "src";
        private String _FilenameAttribute = "alt";

        private readonly ILogger<ImagevenueHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHtmlHandler _htmlHandler;

        public ImagevenueHandler(ILogger<ImagevenueHandler> logger, IHttpClientFactory httpClientFactory, IHtmlHandler htmlHandler)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _htmlHandler = htmlHandler;
        }

        #region Implementation of IImageHostHandler

        /// <inheritdoc />
        public String ImageHost { get; } = "imagevenue.com";

        /// <inheritdoc />
        public async Task<UnitOfWork> HandleUrlAsync(String url)
        {
            return await HandleUrlAsync(url, _httpClientFactory.CreateClient());
        }

        /// <inheritdoc />
        public async Task<UnitOfWork> HandleUrlAsync(String url, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            var document = await _htmlHandler.GetHtmlDocumentAsync(url, httpClient);
            var imgObject = document.QuerySelector(_SelectorMain)?.QuerySelector(_SelectorImage);
            if(imgObject == null)
            {
                throw new ImageHostHandlerException($"[{url}] processing failed - {_SelectorImage} object cannot be null");
            }
            var imageUrl = imgObject.GetAttribute(_UrlAttribute);
            if (String.IsNullOrEmpty(imageUrl))
            {
                throw new ImageHostHandlerException($"[{url}] processing failed - {_UrlAttribute} attribute cannot be null");
            }

            return String.IsNullOrEmpty(imgObject.GetAttribute(_FilenameAttribute)) ? new UnitOfWork(imageUrl) : new UnitOfWork(imageUrl, imgObject.GetAttribute(_FilenameAttribute));
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
            foreach(var url in urls)
            {
                workset.Add(await HandleUrlAsync(url, httpClient));
            }
            request.Add(workset);
            return request;
        }

        #endregion
    }
}
