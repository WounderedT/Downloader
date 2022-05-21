using Shared.Models.Work;

namespace Handlers.Abstractions
{
    /// <summary>
    /// Type provides methods for asynchronous items download.
    /// </summary>
    public interface IDownloadHandler
    {
        /// <summary>
        /// Download handler type.
        /// </summary>
        String DownloaderType { get; }

        /// <summary>
        /// Asynchronously download items from <paramref name="workset"/> into <paramref name="path"/>
        /// </summary>
        /// <param name="workset">Workset to download items from.</param>
        /// <param name="path">Base path to download items to. Combined with WorkSet.FolderName if available.</param>
        /// <param name="skipMissing">Optional flag to ignore missing items from <paramref name="workset"/>. Default value is False</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default value is CancellationToken.None</param>
        /// <returns>
        /// When method completes it returns True if all items from <paramref name="workset"/> were downloaded, including missing items if <paramref name="skipMissing"/>
        /// is set to True, otherwise False.
        /// </returns>
        Task<Boolean> DownloadAsync(WorkSet workset, String path, Boolean skipMissing = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously download items from <paramref name="workset"/> into <paramref name="path"/>
        /// </summary>
        /// <param name="workset">Workset to download items from.</param>
        /// <param name="path">Base path to download items to. Combined with WorkSet.FolderName if available.</param>
        /// <param name="client">Preconfigured instance of HttpClient to use for HTTP requests.</param>
        /// <param name="skipMissing">Optional flag to ignore missing items from <paramref name="workset"/>. Default value is False</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default value is CancellationToken.None</param>
        /// <returns>
        /// When method completes it returns True if all items from <paramref name="workset"/> were downloaded, including missing items if <paramref name="skipMissing"/>
        /// is set to True, otherwise False.
        /// </returns>
        Task<Boolean> DownloadAsync(WorkSet workset, String path, HttpClient client, Boolean skipMissing = false, CancellationToken cancellationToken = default);
    }
}
