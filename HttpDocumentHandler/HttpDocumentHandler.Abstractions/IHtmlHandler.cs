using AngleSharp.Html.Dom;
using HttpDocumentHandler.Abstractions.Exceptions;

namespace HttpDocumentHandler.Abstractions
{
    /// <summary>
    /// Type provides methods to get HTML document.
    /// </summary>
    public interface IHtmlHandler
    {
        /// <summary>
        /// Gets <seealso cref="IHtmlDocument"/> document for provided <paramref name="url"/> using default HTTP client
        /// while monitoring cancellation request.
        /// </summary>
        /// <param name="url">System.String URL page to get html document from.</param>
        /// <param name="cancellationToken">Optional cancellation token. Default value is CancellationToken.None</param>
        /// <returns>When this method completes, it returns a new object (type <seealso cref="IHtmlDocument"/>) represnting html document from <paramref name="url"/>.</returns>
        /// <exception cref="ArgumentNullException">URL or httpClient argument is null.</exception>
        /// <exception cref="HtmlDocumentProcessingException">Exception occured during html document processing.</exception>
        Task<IHtmlDocument> GetHtmlDocumentAsync(String url, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets <seealso cref="IHtmlDocument"/> document for provided <paramref name="url"/> using <paramref name="httpClient"/>
        /// while monitoring cancellation request.
        /// </summary>
        /// <param name="url">System.String URL page to get html document from.</param>
        /// <param name="httpClient"><seealso cref="HttpClient"/> client to use for http requests.</param>
        /// <param name="cancellationToken">Optional cancellation token. Default value is CancellationToken.None</param>
        /// <returns>When this method completes, it returns a new object (type <seealso cref="IHtmlDocument"/>) represnting html document from <paramref name="url"/>.</returns>
        /// <exception cref="ArgumentNullException">URL or httpClient argument is null.</exception>
        /// <exception cref="HtmlDocumentProcessingException">Exception occured during html document processing.</exception>
        Task<IHtmlDocument> GetHtmlDocumentAsync(String url, HttpClient httpClient, CancellationToken cancellationToken = default);
    }
}
