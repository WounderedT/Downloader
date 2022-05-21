using System.Net.Http.Headers;
using Handlers.Processor;
using Handlers.Strategy;

namespace Handlers.DownloadModel
{
    internal interface IDownloadProcessorFactory
    {
        /// <summary>
        /// Gets <see cref="IDownloadProcessor"/> for provided <paramref name="headers"/>
        /// </summary>
        /// <param name="domain">System.String domain to get download model for.</param>
        /// <returns>
        /// Method returns <see cref="IDownloadProcessor"/> for provided <paramref name="domain"/> based configured <see cref="IStrategyFactory"/>.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        IDownloadProcessor GetDownloadProcessor(String domain);

        /// <summary>
        /// Gets <see cref="IDownloadProcessor"/> for provided <paramref name="headers"/>
        /// </summary>
        /// <param name="headers">HttpContentHeaders to get download model for.</param>
        /// <returns>
        /// Method returns <see cref="IDownloadProcessor"/> for provided <paramref name="headers"/> based configured <see cref="IStrategyFactory"/>.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        IDownloadProcessor GetDownloadProcessor(HttpContentHeaders headers);
    }
}
