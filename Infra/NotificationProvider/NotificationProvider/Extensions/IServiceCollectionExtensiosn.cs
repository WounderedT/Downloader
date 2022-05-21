using Microsoft.Extensions.DependencyInjection;
using NotificationProvider.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProvider.Extensions
{
    public static class IServiceCollectionExtensiosn
    {
        public static IServiceCollection ConfigureNotificationProvider(this IServiceCollection services)
        {
            services.AddSingleton<INotificationProvider, NotificationProvider>();
            return services;
        }
    }
}
