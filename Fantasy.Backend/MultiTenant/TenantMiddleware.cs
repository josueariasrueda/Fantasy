namespace Fantasy.Backend.MultiTenant;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        int tenantId = 0; // Valor predeterminado para tenant no identificado

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out int parsedTenantId))
            {
                tenantId = parsedTenantId;
            }
        }

        context.Items["TenantId"] = tenantId; // Almacenar el ID del tenant como int
        await _next(context);
    }
}