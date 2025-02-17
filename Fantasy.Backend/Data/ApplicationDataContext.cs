using Fantasy.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Fantasy.Backend.Data;

public class ApplicationDataContext : IdentityDbContext<User>
{
    // add-migration -Context ApplicationDataContext -o Migrations/ApplicationDBContext InitialCreate
    // update-database -Context ApplicationDataContext

    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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