using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Elders.Cronus.AspNetCore
{
    public static class CronusAspNetCore
    {
        public static IServiceCollection AddCronusAspNetCore(this IServiceCollection services)
        {
            services.AddSingleton<ITenantResolver<DefaultHttpContext>, HttpContextTenantResolver>();
            services.AddSingleton<ITenantResolver<HttpContext>, HttpContextTenantResolver>();
            services.AddSingleton<CronusTenantOptions, CronusTenantOptions>();

            return services;
        }

        public static IServiceCollection AddCronusAspNetCore(this IServiceCollection services, Action<CronusTenantOptions> options)
        {
            services.AddSingleton<ITenantResolver<DefaultHttpContext>, HttpContextTenantResolver>();
            services.AddSingleton<ITenantResolver<HttpContext>, HttpContextTenantResolver>();

            var opts = new CronusTenantOptions();
            options(opts);
            services.AddSingleton<CronusTenantOptions>(opts);

            return services;
        }

        public static IApplicationBuilder UseCronusAspNetCore(this IApplicationBuilder app)
        {
            return app.Use((context, next) =>
            {
                var cronusContextFactory = context.RequestServices.GetRequiredService<CronusContextFactory>();
                CronusContext cronusContext = cronusContextFactory.GetContext(context, context.RequestServices);

                return next.Invoke();
            });
        }
    }

}
