using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Elders.Cronus.AspNetCore
{
    public class CronusControllerFactory : IControllerFactory
    {
        private DefaultControllerFactory defaultControllerFactory;
        private readonly CronusTenantOptions options;

        public CronusControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators)
        {
            defaultControllerFactory = new DefaultControllerFactory(controllerActivator, propertyActivators);
            options = new CronusTenantOptions();
        }

        public CronusControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators, CronusTenantOptions options)
        {
            defaultControllerFactory = new DefaultControllerFactory(controllerActivator, propertyActivators);
            this.options = options;
        }

        public object CreateController(ControllerContext context)
        {
            var cronusContext = context.HttpContext.RequestServices.GetRequiredService<CronusContext>();
            if (cronusContext.IsNotInitialized)
            {
                var tenant = options.TenantResolver?.Resolve(context.HttpContext);
                if (string.IsNullOrEmpty(tenant))
                    tenant = options.DefaultTenant;

                if (string.IsNullOrEmpty(tenant))
                    throw new Exception("Unable to resolve tenant. Make sure that you have `IAspNetTenantResolver` registered. The default implementation is ResolveTenantFromTenantClaim. Another way to hack it is to use `.AddCronusAspNetCore(o => o.DefaultTenant = \"myTenant\")");


                cronusContext.Initialize(tenant, context.HttpContext.RequestServices);
            }

            return defaultControllerFactory.CreateController(context);
        }

        public void ReleaseController(ControllerContext context, object controller) => defaultControllerFactory.ReleaseController(context, controller);
    }

}
