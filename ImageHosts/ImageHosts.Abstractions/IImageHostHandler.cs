using ImageHosts.Abstractions.Exceptions;
using Shared.Models.Work;

namespace ImageHosts.Abstractions
{
    /// <summary>
    /// Type defines methods and properties of image host handler.
    /// </summary>
    public interface IImageHostHandler
    {
        /// <summary>
        /// Read-only name of supported image host.
        /// </summary>
        String ImageHost { get; }

        /// <summary>
        /// Asynchronously processes <paramref name="url"/> using default HTTP client and returns 
        /// new <seealso cref="UnitOfWork"/> object with download url.
        /// </summary>
        /// <param name="url">URL to process.</param>
        /// <returns>When this method completes, it returns a new object (type Shared.Models.Work.UnitOfWork) with download url.</returns>
        /// <exception cref="ArgumentNullException">Input URL is null.</exception>
        /// <exception cref="ImageHostHandlerException">Internal URL processing returned no result.</exception>
        Task<UnitOfWork> HandleUrlAsync(String url);

        /// <summary>
        /// Asynchronously processes <paramref name="url"/> using provided <paramref name="httpClient"/> and returns
        /// new <seealso cref="UnitOfWork"/> object with download url.
        /// </summary>
        /// <param name="url">URL to process.</param>
        /// <param name="httpClient">Configured instance of System.Net.Http.HttpClient to use for HTTP request handling.</param>
        /// <returns>When this method completes, it returns a new object (type Shared.Models.Work.UnitOfWork) with download url.</returns>
        /// <exception cref="ArgumentNullException">Input URL is null.</exception>
        /// <exception cref="ImageHostHandlerException">Internal URL processing returned no result.</exception>
        Task<UnitOfWork> HandleUrlAsync(String url, HttpClient httpClient);

        /// <summary>
        /// Asynchronously processes collection of <paramref name="urls"/> using default HTTP client and returns
        /// new <seealso cref="WorkRequest"/> object with download urls.
        /// </summary>
        /// <param name="urls">Collection of System.String URLs to process.</param>
        /// <returns>When this method completes, it returns a new object (type <seealso cref="WorkRequest"/>) with download urls.</returns>
        /// <exception cref="ArgumentNullException">Input URL is null.</exception>
        /// <exception cref="ImageHostHandlerException">Internal URL processing returned no result.</exception>
        Task<WorkRequest> HandleUrlsAsync(IEnumerable<String> urls);

        /// <summary>
        /// Asynchronously processes collection of <paramref name="urls"/> using provided <paramref name="httpClient"/> and returns
        /// new <seealso cref="WorkRequest"/> object with download urls.
        /// </summary>
        /// <param name="urls">Collection of System.String URLs to process.</param>
        /// <param name="httpClient">Configured instance of System.Net.Http.HttpClient to use for HTTP request handling.</param>
        /// <returns>When this method completes, it returns a new object (type <seealso cref="WorkRequest"/>) with download urls.</returns>
        /// <exception cref="ArgumentNullException">Input URL is null.</exception>
        /// <exception cref="ImageHostHandlerException">Internal URL processing returned no result.</exception>
        Task<WorkRequest> HandleUrlsAsync(IEnumerable<String> urls, HttpClient httpClient);
    }
}
