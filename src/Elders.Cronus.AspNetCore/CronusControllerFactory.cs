using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.AspNetCore
{
    public class CronusControllerFactory : IControllerFactory
    {
        private readonly IControllerFactory controllerFactory;

        public CronusControllerFactory(IControllerFactory controllerFactory)
        {
            this.controllerFactory = controllerFactory;
        }

        public object CreateController(ControllerContext context)
        {
            var cronusContextFactory = context.HttpContext.RequestServices.GetRequiredService<CronusContextFactory>();
            CronusContext cronusContext = cronusContextFactory.GetContext(context.HttpContext, context.HttpContext.RequestServices);

            return controllerFactory.CreateController(context);
        }

        public void ReleaseController(ControllerContext context, object controller) => controllerFactory.ReleaseController(context, controller);
    }
}
