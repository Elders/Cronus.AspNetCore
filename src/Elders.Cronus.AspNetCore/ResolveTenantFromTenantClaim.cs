using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Elders.Cronus.AspNetCore
{
    public class HttpContextTenantResolver : ITenantResolver<DefaultHttpContext>, ITenantResolver<HttpContext>
    {
        private readonly ITenantList tenantList;

        public HttpContextTenantResolver(ITenantList tenantList)
        {
            this.tenantList = tenantList;
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
                if (tenantList.GetTenants().Count() == 1)
                {
                    tenant = tenantList.GetTenants().Single();
                }
                else
                {
                    throw new Exception("Unable to resolve tenant. Make sure that you have `IAspNetTenantResolver` registered. The default implementation is ResolveTenantFromTenantClaim.");
                }
            }

            return tenant;
        }
    }
}
