using System;
using System.Linq;
using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Elders.Cronus.AspNetCore
{
    public class HttpContextTenantResolver : ITenantResolver<DefaultHttpContext>, ITenantResolver<HttpContext>
    {
        public HttpContextTenantResolver() { }

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
            string tenant = string.Empty;

            if (context.User.Identity.IsAuthenticated == false)
                return tenant;

            if (context is null || context.User is null || context.User.Claims.Any() == false)
            {
                // resolve tenant from header in case of authenticationless process
                context.Request.Headers.TryGetValue("tenant", out StringValues tenantHeader);
                tenant = tenantHeader.FirstOrDefault(t => string.IsNullOrEmpty(t) == false);

                return tenant;
            }

            // resolve tenant from jwt token
            var tenantClaim = context.User.Claims.Where(claim => claim.Type.Equals("tenant", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("tenant_client", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("client_tenant", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            tenant = tenantClaim?.Value;

            return tenant;
        }
    }
}
