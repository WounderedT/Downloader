using DomainProcessors.Abstractions;
using DomainProcessors.Configuration;
using DomainProcessors.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainProcessors.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDomainProcessors(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<ReactorProcessorConfig>(configurationSection.GetSection("joyreactor.cc"));
            services.AddSingleton<IDomainProcessor, DefaultProcessor>();
            services.AddSingleton<IDomainProcessor, BunkrProcessor>();
            services.AddSingleton<IDomainProcessor, CyberdropProcessor>();
            services.AddSingleton<IDomainProcessor, ReactorProcessor>();
            services.AddSingleton<IDomainProcessorFactory, DomainProcessorFactrory>();
            return services;
        }
    }
}
