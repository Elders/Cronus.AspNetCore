using System;
using System.Linq;
using Elders.Cronus.AspNetCore.Exeptions;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (context.User is null) throw new UnableToResolveTenantExeption($"Unable to resolve tenant. There is no ClaimsPrincipal. {nameof(HttpContext.User)}");
            if (context.User.Claims.Any() == false)
                throw new UnableToResolveTenantExeption("Unable to resolve tenant. Claims collection is empty. Did you configure the correct authority and audience?");

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
                    //throw new Exception($"Unable to resolve tenant. Make sure that you have `IAspNetTenantResolver` registered. The default implementation is {nameof(HttpContextTenantResolver)}.");
                    throw new UnableToResolveTenantExeption("Unable to resolve tenant.");
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
