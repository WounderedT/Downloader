namespace DomainProcessors.Abstractions
{
    /// <summary>
    /// Type defines methods to get defined domain processors based on providede URL.
    /// </summary>
    public interface IDomainProcessorFactory
    {
        /// <summary>
        /// Returns IDomainProcessor defined for <paramref name="url"/> domain, or registered defailt processor if no <paramref name="url"/> specific processor found.
        /// </summary>
        /// <param name="url">URL to get processor for.</param>
        /// <returns>An instance of IDomainProcessor defined for <paramref name="url"/>, otherwise an instance of default registered IDomainProcessor.</returns>
        IDomainProcessor GetDomainProcessor(String url);
    }
}
