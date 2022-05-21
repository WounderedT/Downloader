using DomainProcessors.Abstractions.Exceptions;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Abstractions
{
    /// <summary>
    /// Type defines methods and properties of a domain processor.
    /// </summary>
    public interface IDomainProcessor
    {
        /// <summary>
        /// Gets supported domain name.
        /// </summary>
        String Domain { get; }

        /// <summary>
        /// Gets supported domain name regex.
        /// </summary>
        Regex? DomainRegex { get; }

        /// <summary>
        /// Attempts to retrieve image count asynchronously.
        /// </summary>
        /// <param name="url">An URL to image galery.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default value is CancellationToken.None</param>
        /// <returns>When this method completes, it returns galery images count or -1 if image count is unavalable.</returns>
        Task<Int32> GetElementsCountAsync(String url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously processes image galery URL and returns a WorkRequest object with processed image urls.
        /// </summary>
        /// <param name="url">An URL to image galery.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default value is CancellationToken.None</param>
        /// <returns>When this method completes, it returns a new object (type WorkRequest) with processed image urls.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is <see langword="null"/> or empty string.</exception>
        /// <exception cref="ArgumentException">Unsupporter <paramref name="url"/> provided.</exception>
        /// <exception cref="UrlProcessingException">Retriable URL processing failure due to external error.</exception>
        /// <exception cref="IParentNodeProcessingException">Non-retriable URL processing failed due to internal exception.</exception>
        /// <exception cref="InternalUrlProcessingException">Non-retriable URL processing failed due to internal exception.</exception>
        Task<WorkRequest> ProcessAsync(String url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously processes image galery URLs and returns a new WorkRequest object with processed image urls.
        /// </summary>
        /// <param name="urls">Image galery URLs</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default value is CancellationToken.None</param>
        /// <returns>When this method completes, it returns a new object (type WorkRequest) with processed image urls</returns>
        /// <exception cref="ArgumentNullException"><paramref name="urls"/> is <see langword="null"/> or empty string.</exception>
        /// <exception cref="ArgumentException">Unsupporter <paramref name="urls"/> provided.</exception>
        /// <exception cref="UrlProcessingException">Retriable URL processing failure due to external error.</exception>
        /// <exception cref="IParentNodeProcessingException">Non-retriable URL processing failed due to internal exception.</exception>
        /// <exception cref="InternalUrlProcessingException">Non-retriable URL processing failed due to internal exception.</exception>
        Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default);
    }
}
