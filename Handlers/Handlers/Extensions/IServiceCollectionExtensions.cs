using Handlers.Abstractions;
using Handlers.ConcurrentProcessing;
using Handlers.Configuration;
using Handlers.DownloadModel;
using Handlers.Processor;
using Handlers.Strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Handlers.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDownloadHadlers(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<DownloadHandlerConfig>(configurationSection);
            services.AddHttpClient();
            services.AddSingleton<IStrategyFactory, StrategyFactory>();
            services.AddSingleton<IDownloadProcessorFactory, DownloadProcessorFactory>();
            services.AddSingleton<IConcurrentExecutionProcessor, ConcurrentExecutionProcessor>();
            services.AddSingleton<IDownloadProcessor, BaseDownloadProcessor>();
            services.AddSingleton<IDownloadProcessor, FragmentedDownloadProcessor>();
            services.AddSingleton<IDownloadHandler, DefaultDownloadHandler>();
            services.AddSingleton<IDownloadHandlerFactory, DownloadHandlerFactory>();
            return services;
        }
    }
}
