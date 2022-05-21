using DomainProcessors.Abstractions;

namespace DomainProcessors.Extensions.UI.Interfaces
{
    /// <summary>
    /// Type extends IDomainProcessor to add support for WebView based processing.
    /// </summary>
    public interface IDomainUIProcessor : IDomainProcessor
    {
        /// <summary>
        /// Gets or sets WebView object.
        /// </summary>
        public Object WebViewObj { get; set; } //Could be handled by DI in constructor?
    }
}
