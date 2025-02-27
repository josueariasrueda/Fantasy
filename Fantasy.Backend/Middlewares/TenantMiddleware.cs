namespace Fantasy.Backend.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string tenant = "default-tenant";

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = context.User.FindFirst("tenant");
                if (tenantClaim != null)
                {
                    tenant = tenantClaim.Value;
                }
            }

            context.Items["Tenant"] = tenant;
            await _next(context);
        }
    }
}