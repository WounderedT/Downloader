using Shared.Models.Result;

namespace DownloaderCli.Interfaces
{
    /// <summary>
    /// Type provide methods to facilitate items download from provided URLs.
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// Asynchronously download items from provided URLs.
        /// </summary>
        /// <param name="urls">URLs to process and download items from.</param>
        /// <param name="path">Base directory path to download items to.</param>
        /// <returns>A <see cref="DownloadResult"/> object containing result of the asynchronous download operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<DownloadResult> DownloadAsync(IEnumerable<String> urls, String path);

        /// <summary>
        /// Asynchronously download items from provided URL.
        /// </summary>
        /// <param name="url">URL to process and download items from.</param>
        /// <param name="path">Base directory path to download items to.</param>
        /// <returns>A <see cref="DownloadResult"/> object containing result of the asynchronous download operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<DownloadResult> DownloadAsync(String url, String path);

        /// <summary>
        /// Asynchronously retries download from provided <paramref name="originalResult"/>.
        /// </summary>
        /// <param name="originalResult">Original download result to retry.</param>
        /// <param name="path">Base directory path to download items to.</param>
        /// <param name="force">Optional parameter to retry successfull download. Default value is False.</param>
        /// <returns>A new <see cref="DownloadResult"/> object containing result of the asynchronous download operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<DownloadResult> RetryAsync(DownloadResult originalResult, String path, Boolean force = false);
    }
}
