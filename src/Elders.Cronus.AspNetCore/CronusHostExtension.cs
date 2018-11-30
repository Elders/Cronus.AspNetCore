using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.AspNetCore
{
    public static class CronusExtension
    {
        public static IApplicationBuilder UseCronus(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var cronusHost = serviceProvider.GetService<ICronusHost>();
            var tenants = serviceProvider.GetService<ITenantList>();


            foreach (var tenant in tenants.GetTenants())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    CronusContext context = scope.ServiceProvider.GetRequiredService<CronusContext>();
                    context.Initialize(tenant, scope.ServiceProvider);
                }
            }


            cronusHost.Start();

            return app;
        }

        public static IApplicationBuilder StopCronus(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var cronusHost = serviceProvider.GetService<ICronusHost>();

            cronusHost.Stop();

            return app;
        }
    }

}
