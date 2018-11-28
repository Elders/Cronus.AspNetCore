using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Elders.Cronus.AspNetCore
{
    public class ResolveTenantFromTenantClaim : IAspNetTenantResolver
    {
        public string Resolve(HttpContext context)
        {
            var tenantClaim = context.User.Claims.Where(claim => claim.Type.Equals("tenant", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("tenant_client", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("client_tenant", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            return tenantClaim?.Value;
        }
    }

}
