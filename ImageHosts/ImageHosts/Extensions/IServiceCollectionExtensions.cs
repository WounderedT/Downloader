using ImageHosts.Abstractions;
using ImageHosts.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ImageHosts.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDownloaderImageHosts(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IImageHostHandler, DefaultHanlder>();
            serviceCollection.AddSingleton<IImageHostHandler, ImagevenueHandler>();
            serviceCollection.AddSingleton<IImageHostHandler, FastpicHandler>();
            serviceCollection.AddSingleton<IImageHostHandlerFactory, ImageHostHandlerFactory>();
            return serviceCollection;
        }
    }
}
