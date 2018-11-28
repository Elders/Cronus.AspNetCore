namespace Elders.Cronus.AspNetCore
{
    public class CronusTenantOptions
    {
        public CronusTenantOptions()
        {

        }

        public CronusTenantOptions(IAspNetTenantResolver tenantResolver)
        {
            TenantResolver = tenantResolver;
        }

        public string DefaultTenant { get; set; }

        public IAspNetTenantResolver TenantResolver { get; set; }
    }

}
