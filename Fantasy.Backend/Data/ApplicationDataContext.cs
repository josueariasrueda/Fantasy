using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Fantasy.Backend.Data;

public class ApplicationDataContext : IdentityDbContext<User>
{
    // add-migration -Context ApplicationDataContext -o Migrations/ApplicationDBContext InitialCreate
    // update-database -Context ApplicationDataContext
    // remove-migration -Context ApplicationDataContext
    // Drop-Database -Context "ApplicationDataContext"

    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
    {
    }

    public DbSet<Module> Modules { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Enterprise> Enterprises { get; set; }
    public DbSet<UserTenantPermission> UsersTenantPermissions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<AccountingAccount> AccountingAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTenantPermission>()
            .HasKey(utp => new { utp.UserId, utp.TenantId, utp.ModuleId });

        modelBuilder.Entity<UserTenantPermission>()
            .HasOne(utp => utp.User)
            .WithMany(u => u.UsersTenantPermissions)
            .HasForeignKey(utp => utp.UserId)
            .IsRequired();

        modelBuilder.Entity<UserTenantPermission>()
            .HasOne(utp => utp.Tenant)
            .WithMany(t => t.UsersTenantPermissions)
            .HasForeignKey(utp => utp.TenantId)
            .IsRequired();

        // Configurar la relación entre EnterpriseTenant y Tenant
        modelBuilder.Entity<EnterpriseTenant>()
            .HasOne(et => et.Tenant) // Cada EnterpriseTenant tiene un Tenant
            .WithMany(t => t.TenantsEnterprises) // Un Tenant puede tener muchas relaciones con EnterpriseTenant
            .HasForeignKey(et => et.TenantId) // Clave foránea en EnterpriseTenant
            .OnDelete(DeleteBehavior.Restrict); // Configurar el comportamiento al eliminar un Tenant

        // Configurar la relación entre EnterpriseTenant y Enterprise
        modelBuilder.Entity<EnterpriseTenant>()
            .HasOne(et => et.Enterprise) // Cada EnterpriseTenant tiene una Enterprise
            .WithMany(e => e.EnterprisesTenants) // Una Enterprise puede tener muchas relaciones con EnterpriseTenant
            .HasForeignKey(et => et.EnterpriseId) // Clave foránea en EnterpriseTenant
            .OnDelete(DeleteBehavior.Restrict); // Configurar el comportamiento al eliminar una Enterprise

        modelBuilder.Entity<UserTenantPermission>()
            .HasOne(utp => utp.Module)
            .WithMany(m => m.UsersTenantPermissions)
            .HasForeignKey(utp => utp.ModuleId)
            .IsRequired();

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Tenant)
            .WithMany(t => t.Subscriptions)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Country>()
            .HasOne(c => c.DefaultCurrency)
            .WithMany()
            .HasForeignKey(c => c.DefaultCurrencyId)
            .IsRequired();

        modelBuilder.Entity<AccountingAccount>()
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Restrict); // Configura el comportamiento al eliminar un tenant

        modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Country>().HasIndex(c => c.Code).IsUnique();

        DisableCascadingDelete(modelBuilder);
    }

    private void DisableCascadingDelete(ModelBuilder modelBuilder)
    {
        var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
        foreach (var relationship in relationships)
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}