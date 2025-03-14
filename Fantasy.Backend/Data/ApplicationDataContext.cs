﻿using Fantasy.Shared.Entities.Domain;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Enterprise>()
            .HasOne<Tenant>()
            .WithMany(t => t.Enterprises)
            .HasForeignKey(e => e.TenantId)
            .IsRequired();

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

        modelBuilder.Entity<UserTenantPermission>()
            .HasOne(utp => utp.Module)
            .WithMany(m => m.UsersTenantPermissions)
            .HasForeignKey(utp => utp.ModuleId)
            .IsRequired();

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .IsRequired();

        modelBuilder.Entity<Country>()
            .HasOne(c => c.DefaultCurrency)
            .WithMany()
            .HasForeignKey(c => c.DefaultCurrencyId)
            .IsRequired();

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