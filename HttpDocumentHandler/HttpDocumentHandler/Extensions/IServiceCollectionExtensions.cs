using HttpDocumentHandler.Abstractions;
using HttpDocumentHandler.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace HttpDocumentHandler.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureHtmlDocumentHandler(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IHtmlDocumentCache, HtmlDocumentCache>();
            serviceCollection.AddSingleton<IHtmlHandler, HtmlHandler>();
            return serviceCollection;
        }
    }
}
