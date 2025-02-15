using Fantasy.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Fantasy.Backend.Data;

public class ApplicationDataContext : DbContext
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
    }
}