using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Elders.Cronus.AspNetCore
{
    public class HttpContextTenantResolver : ITenantResolver<DefaultHttpContext>, ITenantResolver<HttpContext>
    {
        private TenantsOptions tenants;

        public HttpContextTenantResolver(IOptionsMonitor<TenantsOptions> tenantsOptions)
        {
            this.tenants = tenantsOptions.CurrentValue;
            tenantsOptions.OnChange(Changed);
        }

        public string Resolve(DefaultHttpContext source)
        {
            return ResolveTenant(source);
        }

        public string Resolve(HttpContext source)
        {
            return ResolveTenant(source);
        }

        private string ResolveTenant(HttpContext context)
        {
            var tenantClaim = context.User.Claims.Where(claim => claim.Type.Equals("tenant", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("tenant_client", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("client_tenant", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            string tenant = tenantClaim?.Value;

            if (string.IsNullOrEmpty(tenant))
            {
                if (tenants.Tenants.Count() == 1)
                {
                    tenant = tenants.Tenants.Single();
                }
                else
                {
                    throw new Exception("Unable to resolve tenant. Make sure that you have `IAspNetTenantResolver` registered. The default implementation is ResolveTenantFromTenantClaim.");
                }
            }

            return tenant;
        }

        private void Changed(TenantsOptions newTenants)
        {
            if (tenants != newTenants)
            {
                tenants = newTenants;
            }
        }
    }
}
