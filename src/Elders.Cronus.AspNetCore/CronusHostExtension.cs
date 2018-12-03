using Elders.Cronus.AspNetCore.Logging;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Multitenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Cronus.AspNetCore
{
    public static class CronusExtension
    {
        public static IServiceCollection UseCronus(this IServiceCollection services)
        {
            services.AddHostedService<CronusHostService>();

            return services;
        }
    }



    public class CronusHostService : IHostedService, IDisposable
    {
        private static readonly ILog log = LogProvider.GetLogger(typeof(CronusHostService));

        private readonly IConfiguration configuration;
        private readonly ICronusHost cronusHost;

        public CronusHostService(IConfiguration configuration, IServiceProvider provider, ITenantList tenants, ICronusHost cronusHost)
        {
            this.configuration = configuration;
            this.cronusHost = cronusHost;

            foreach (var tenant in tenants.GetTenants())
            {
                using (var scope = provider.CreateScope())
                {
                    CronusContext context = scope.ServiceProvider.GetRequiredService<CronusContext>();
                    context.Initialize(tenant, scope.ServiceProvider);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.Info("Starting Cronus Host service...");

            cronusHost.Start();

            log.Info("Service Cronus Host started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.Info("Stopping Cronus Host service...");

            cronusHost.Stop();

            log.Info("Service Cronus Host stopped");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }

}
