using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Elders.Cronus.AspNetCore
{
    public static class CronusAspNetCore
    {
        public static IServiceCollection AddCronusAspNetCore(this IServiceCollection services)
        {
            services.AddSingleton<IControllerFactory, CronusControllerFactory>();
            services.AddSingleton<CronusTenantOptions, CronusTenantOptions>();

            return services;
        }

        public static IServiceCollection AddCronusAspNetCore(this IServiceCollection services, Action<CronusTenantOptions> options)
        {
            services.AddSingleton<IControllerFactory, CronusControllerFactory>();

            var opts = new CronusTenantOptions();
            options(opts);
            services.AddSingleton<CronusTenantOptions>(opts);

            return services;
        }
    }

}
