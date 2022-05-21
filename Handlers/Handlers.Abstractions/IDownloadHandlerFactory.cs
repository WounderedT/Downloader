namespace Handlers.Abstractions
{
    /// <summary>
    /// Type provide methods to get download handlers.
    /// </summary>
    public interface IDownloadHandlerFactory
    {
        /// <summary>
        /// Gets registered IDownloadHandler implementation for providede <paramref name="downloaderType"/>
        /// </summary>
        /// <param name="downloaderType">Type of downlader to get handler for.</param>
        /// <returns>An instance of IDownloadHandler if download handler is registered for <paramref name="downloaderType"/>, otherwise null.</returns>
        IDownloadHandler? GetHandler(String downloaderType);

        /// <summary>
        /// Gets registered dafault IDownloadHandler implementation.
        /// </summary>
        /// <returns>An instance of type registered as default implementation of IDownloadHandler.</returns>
        IDownloadHandler GetDefaultHandler();
    }
}
