using Elders.Cronus.AspNetCore.Exeptions;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Elders.Cronus.AspNetCore
{
    public static class CronusAspNetCore
    {
        public static IServiceCollection AddCronusAspNetCore(this IServiceCollection services)
        {
            services.AddSingleton<ITenantResolver<DefaultHttpContext>, HttpContextTenantResolver>();
            services.AddSingleton<ITenantResolver<HttpContext>, HttpContextTenantResolver>();
            services.AddSingleton<HttpContextTenantResolver>();

            return services;
        }

        public static IApplicationBuilder UseCronusAspNetCore(this IApplicationBuilder app)
        {
            return app.Use((context, next) => ResolveCronusContext(context, next));
        }

        public static IApplicationBuilder UseCronusAspNetCore(this IApplicationBuilder app, Func<HttpContext, bool> shouldResolveTenant)
        {
            return app.Use((context, next) =>
            {
                bool shouldResolve = true;
                if (shouldResolveTenant is null == false)
                    shouldResolve = shouldResolveTenant(context);

                if (shouldResolve)
                {
                    return ResolveCronusContext(context, next);
                }

                return next.Invoke();
            });
        }

        private static Task ResolveCronusContext(HttpContext context, Func<Task> next)
        {
            try
            {
                var cronusContextFactory = context.RequestServices.GetRequiredService<CronusContextFactory>();
                CronusContext cronusContext = cronusContextFactory.GetContext(context, context.RequestServices);

                return next.Invoke();
            }
            catch (UnableToResolveTenantException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return Task.CompletedTask;
            }
        }
    }
}
