using Elders.Cronus.AspNetCore.Exceptions;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            return app.Use((context, next) =>
            {
                bool shouldResolve = ShouldResolveTenant(context);
                if (shouldResolve)
                    return ResolveCronusContext(context, next);

                return next.Invoke();
            });
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
                var cronusContextFactory = context.RequestServices.GetRequiredService<DefaultCronusContextFactory>();
                CronusContext cronusContext = cronusContextFactory.Create(context, context.RequestServices);

                ILogger logger = context.RequestServices.GetService<ILogger>();
                using (logger.BeginScope(s => s.AddScope(Log.Tenant, cronusContext.Tenant)))
                {
                    return next.Invoke();
                }
            }
            catch (UnableToResolveTenantException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return Task.CompletedTask;
            }
        }

        private static bool ShouldResolveTenant(HttpContext context)
        {
            bool shoudResolve = true;

            Endpoint endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint is not null)
            {
                BypassTenantAttribute doNotRequireTenantAttribute = endpoint.Metadata.GetMetadata<BypassTenantAttribute>();
                if (doNotRequireTenantAttribute is not null)
                    shoudResolve = false;
            }
            return shoudResolve;
        }
    }
}
