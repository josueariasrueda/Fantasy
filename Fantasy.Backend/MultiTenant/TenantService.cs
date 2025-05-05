using Fantasy.Backend.Data;
using Fantasy.Shared.Entities.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.MultiTenant;

public interface ITenantService
{
    Task CreateNewTenantAsync(string tenantName, string connectionString);

    Task CreateTenantDatabaseAsync(Tenant tenant);

    Task SeedTenantDatabaseAsync(DataContext tenantContext);

    Tenant? GetTenantById(int tenantId);
}

public class TenantService : ITenantService
{
    private readonly ApplicationDataContext _applicationContext;

    public TenantService(ApplicationDataContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task CreateNewTenantAsync(string tenantName, string connectionString)
    {
        var tenant = new Tenant
        {
            Name = tenantName,
            ConnectionString = connectionString,
            IsActive = true
        };

        _applicationContext.Tenants.Add(tenant);
        await _applicationContext.SaveChangesAsync();

        var tenantService = new TenantService(_applicationContext);
        await tenantService.CreateTenantDatabaseAsync(tenant);
    }

    public async Task CreateTenantDatabaseAsync(Tenant tenant)
    {
        // Crear la base de datos del tenant
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(tenant.ConnectionString);

        using var tenantContext = new DataContext(optionsBuilder.Options);
        await tenantContext.Database.EnsureCreatedAsync();

        // Replicar los datos iniciales
        await SeedTenantDatabaseAsync(tenantContext);
    }

    public async Task SeedTenantDatabaseAsync(DataContext tenantContext)
    {
        // Replicar módulos
        var modules = await _applicationContext.Modules.ToListAsync();
        tenantContext.Modules.AddRange(modules);

        // Replicar roles
        //var roles = await _applicationContext.Roles.ToListAsync();
        //tenantContext.Roles.AddRange(roles);

        // Replicar países y monedas
        var countries = await _applicationContext.Countries.ToListAsync();
        tenantContext.Countries.AddRange(countries);

        var currencies = await _applicationContext.Currencies.ToListAsync();
        tenantContext.Currencies.AddRange(currencies);

        await tenantContext.SaveChangesAsync();
    }

    public Tenant? GetTenantById(int tenantId)
    {
        return _applicationContext.Tenants.FirstOrDefault(t => t.TenantId == tenantId);
    }
}