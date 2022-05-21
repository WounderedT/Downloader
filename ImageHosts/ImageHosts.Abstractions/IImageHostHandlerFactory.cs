namespace ImageHosts.Abstractions
{
    /// <summary>
    /// Type contains methods of image host handler factory.
    /// </summary>
    public interface IImageHostHandlerFactory
    {
        /// <summary>
        /// Gets a registered <see cref="IImageHostHandler"/> for provided <paramref name="url"/>.
        /// </summary>
        /// <param name="url">System.String URL to return registered instance of <see cref="IImageHostHandler"/> for.</param>
        /// <returns>
        /// A registered <see cref="IImageHostHandler"/> for provided <paramref name="url"/>, or default <see cref="IImageHostHandler"/>, if
        /// no <see cref="IImageHostHandler"/> has been registered. 
        /// </returns>
        /// <exception cref="NotImplementedException">No registered image host handlers found.</exception>
        /// <exception cref="ArgumentNullException">Input <paramref name="url"/> is either <see langword="null"/> or an empty string.</exception>
        IImageHostHandler GetHandler(String url);
    }
}
