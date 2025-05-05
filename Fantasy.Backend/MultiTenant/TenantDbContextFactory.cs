using Fantasy.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.MultiTenant;

public class TenantDbContextFactory
{
    private readonly ApplicationDataContext _applicationContext;

    public TenantDbContextFactory(ApplicationDataContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public DataContext CreateTenantContext(int tenantId)
    {
        var tenant = _applicationContext.Tenants.FirstOrDefault(t => t.TenantId == tenantId);
        if (tenant == null)
        {
            throw new Exception("Tenant not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(tenant.ConnectionString);

        return new DataContext(optionsBuilder.Options);
    }
}