using Microsoft.AspNetCore.Http;

namespace Elders.Cronus.AspNetCore
{
    public interface IAspNetTenantResolver
    {
        string Resolve(HttpContext context);
    }

}
