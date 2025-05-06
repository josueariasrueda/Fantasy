using Fantasy.Backend.Data;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories;

public interface ICurrenciesRepository
{
    Task<ActionResponse<Currency>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Currency>>> GetAsync();

    Task<ActionResponse<IEnumerable<Currency>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Currency>> GetComboAsync();
}

public class CurrenciesRepository : GenericRepository<Currency>, ICurrenciesRepository
{
    private readonly AppDataContext _context;

    public CurrenciesRepository(AppDataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<Currency>>> GetAsync()
    {
        var currencies = await _context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Currency>>
        {
            WasSuccess = true,
            Result = currencies
        };
    }

    public async Task<ActionResponse<IEnumerable<Currency>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Currencies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Currency>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)
                .Take(pagination.RecordsNumber)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Currencies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Currency>> GetAsync(int id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null)
        {
            return new ActionResponse<Currency>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Currency>
        {
            WasSuccess = true,
            Result = currency
        };
    }

    public async Task<IEnumerable<Currency>> GetComboAsync()
    {
        return await _context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}