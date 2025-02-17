using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Fantasy.Backend.Data;

[ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
public class SeedApplicationDbContext
{
    private readonly ApplicationDataContext _context;

    public SeedApplicationDbContext(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesStatesCitiesSQLScript = File.ReadAllText("Data\\Scripts\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesStatesCitiesSQLScript);
        }
    }
}