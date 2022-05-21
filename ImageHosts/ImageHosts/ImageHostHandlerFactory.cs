using ImageHosts.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ImageHosts
{
    internal static class Constants
    {
        internal const String DefaultHandler = "Default";
    }

    internal class ImageHostHandlerFactory : IImageHostHandlerFactory
    {
        private const String DomainPattern = "https?://([a-zA-Z0-9\\.%_~-]*)/*";

        private readonly Regex _domainRegex;
        private readonly ILogger<ImageHostHandlerFactory> _logger;
        private readonly Dictionary<String, IImageHostHandler> _handlersByHost;

        public ImageHostHandlerFactory(IEnumerable<IImageHostHandler> imageHandlers, ILogger<ImageHostHandlerFactory> logger)
        {
            _logger = logger;
            _handlersByHost = imageHandlers.ToDictionary(handler => handler.ImageHost, handler => handler);
            _domainRegex = new Regex(DomainPattern);
        }

        #region Implementation of IImageHostHandlerManager

        /// <inheritdoc />
        public IImageHostHandler GetHandler(String url)
        {
            if (_handlersByHost == null || _handlersByHost.Count == 0)
            {
                throw new NotImplementedException("No registered image host handlers found!");
            }
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (_handlersByHost.TryGetValue(GetDomain(url), out IImageHostHandler? handler))
            {
                return handler;
            }
            if (_handlersByHost.TryGetValue(Constants.DefaultHandler, out handler))
            {
                return handler;
            }
            throw new NotImplementedException("No registered default image host handlers found!");
        }

        #endregion

        private String GetDomain(String url)
        {
            var domain = new Uri(url).Host;
            return domain.Replace("www.", "");
            //Match? match = _domainRegex.Match(url);
            //if (match.Success)
            //{
            //    return match.Groups[1].Value;
            //}

            //_logger.LogError($"Failed to get domain name from URL [{url}]");
            //throw new ImageHostHandlerException($"Failed to get domain name from URL [{url}]");
        }
    }
}
