using DownloaderCli.Configuration;
using DownloaderCli.Interfaces;
using DownloaderCli.Resume;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DownloaderCli.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Extension method to register DownloaderCli services in IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the services to.</param>
        /// <param name="configurationSection">Microsoft.Extensions.Configuration.IConfigurationSection object to configure DownloaderCli services.</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureDownloaderCli(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<DownloaderCliConfig>(configurationSection);
            services.AddSingleton<IResumeHandler, ResumeHandler>();
            services.AddSingleton<IDownloader, Downloader>();
            return services;
        }
    }
}
