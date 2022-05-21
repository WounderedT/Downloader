using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProgressHandler.Abstractions;
using ProgressHandler.Configuration;

namespace ProgressHandler.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProgressHandler(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<ProgressHandlerConfig>(configurationSection);
            services.AddSingleton<IProgressHandler, ProgressHandler>();
            services.AddSingleton<IPopupNotificationProvider, ToastNotificationProvider>();
            return services;
        }
    }
}
