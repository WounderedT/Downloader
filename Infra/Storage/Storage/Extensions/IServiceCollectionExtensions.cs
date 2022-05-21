using Infra.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Storage.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureStorageProvider(this IServiceCollection services)
        {
            services.AddSingleton<IStorageProvider, StorageProvider>();
            return services;
        }
    }
}
