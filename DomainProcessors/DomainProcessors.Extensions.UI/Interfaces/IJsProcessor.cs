using Shared.Models.Work;

namespace DomainProcessors.Extensions.UI.Interfaces
{
    /// <summary>
    /// Type defines methods for domain processing using JavaScript code snippets.
    /// </summary>
    public interface IJsProcessor
    {
        Task<WorkSet> ProcessAsync(Object webView, String jsCodeSnippet); //webView type should be WebView2 or an interface (custom?).
    }
}
