using System;
using System.Linq;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;

namespace Elders.Cronus.AspNetCore
{
    public class HttpContextTenantResolver : ITenantResolver<DefaultHttpContext>, ITenantResolver<HttpContext>
    {
        public HttpContextTenantResolver()
        {

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
            if (context is null || context.User is null || context.User.Claims.Any() == false)
                return string.Empty;

            var tenantClaim = context.User.Claims.Where(claim => claim.Type.Equals("tenant", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("tenant_client", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("client_tenant", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            string tenant = tenantClaim?.Value;

            return tenant;
        }
    }
}
