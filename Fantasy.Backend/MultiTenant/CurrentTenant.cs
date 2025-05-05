using Fantasy.Shared.Entities.Infraestructure;

namespace Fantasy.Backend.MultiTenant;

public interface ICurrentTenant
{
    int GetCurrentTenantId();

    Tenant? GetCurrentTenant();
}

public class CurrentTenant : ICurrentTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantService _tenantService;

    public CurrentTenant(IHttpContextAccessor httpContextAccessor, ITenantService tenantService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantService = tenantService;
    }

    public int GetCurrentTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Items.TryGetValue("TenantId", out var tenantIdObj) && tenantIdObj is int tenantId)
        {
            return tenantId;
        }
        return 0;
    }

    public Tenant? GetCurrentTenant()
    {
        var tenantId = GetCurrentTenantId();
        if (tenantId == 0)
            return null;

        // Se asume que TenantService tiene un método para obtener el tenant por id
        return _tenantService.GetTenantById(tenantId);
    }
}