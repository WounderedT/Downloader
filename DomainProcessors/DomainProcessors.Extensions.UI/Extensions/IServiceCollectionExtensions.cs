using DomainProcessors.Abstractions;
using DomainProcessors.Extensions.UI.Interfaces;
using DomainProcessors.Extensions.UI.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace DomainProcessors.Extensions.UI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDomainUIProcessors(this IServiceCollection services)
        {
            services.AddSingleton<IJsProcessor, JsProcessor>();
            services.AddSingleton<IDomainProcessor, BellazonProcessor>();
            services.AddSingleton<IDomainProcessor, ErosberryProcessor>();
            services.AddSingleton<IDomainProcessor, FapopediaProcessor>();
            services.AddSingleton<IDomainProcessor, NcfProcessor>();
            return services;
        }
    }
}
