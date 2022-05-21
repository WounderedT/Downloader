using Handlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Handlers
{
    ///<inheritdoc/>
    internal class DownloadHandlerFactory : IDownloadHandlerFactory
    {
        private readonly ILogger<DownloadHandlerFactory> _logger;
        private readonly Dictionary<String, IDownloadHandler> _handlersByHost;

        public DownloadHandlerFactory(IEnumerable<IDownloadHandler> downloadHandler, ILogger<DownloadHandlerFactory> logger)
        {
            _logger = logger;
            _handlersByHost = downloadHandler.ToDictionary(handler => handler.DownloaderType, handler => handler);
        }

        ///<inheritdoc/>
        public IDownloadHandler? GetHandler(String downloaderType)
        {
            if (String.IsNullOrEmpty(downloaderType))
            {
                throw new ArgumentNullException(nameof(downloaderType));
            }

            _handlersByHost.TryGetValue(downloaderType, out IDownloadHandler? handler);
            return handler;
        }

        ///<inheritdoc/>
        public IDownloadHandler GetDefaultHandler()
        {
            IDownloadHandler? handler = GetHandler(Constants.DefaultDownloaderType);
            return handler != null ? handler : throw new NotImplementedException("Default handler is not defined");
        }
    }
}
