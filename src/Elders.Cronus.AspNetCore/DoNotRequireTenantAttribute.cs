using System;

namespace Elders.Cronus.AspNetCore
{
    /// <summary>
    /// An attribute which can be used to specify that the current request does not require a tenant
    /// Keep in mind that cronus context requires tenant, so use at your own discretion
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DoNotRequireTenant : Attribute
    {
    }
}
