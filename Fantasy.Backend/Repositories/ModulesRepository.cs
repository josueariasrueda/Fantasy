using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories;

public interface IModulesRepository
{
    Task<ActionResponse<IEnumerable<Module>>> GetAsync();

    Task<ActionResponse<Module>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Module>> GetComboAsync();
}

public class ModulesRepository : GenericRepository<Module>, IModulesRepository
{
    private readonly ApplicationDataContext _context;

    public ModulesRepository(ApplicationDataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Module>>> GetAsync()
    {
        var modules = await _context.Modules
            .OrderBy(m => m.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Module>>
        {
            WasSuccess = true,
            Result = modules
        };
    }

    public override async Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Modules.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Module>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Modules.AsQueryable();

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

    public override async Task<ActionResponse<Module>> GetAsync(int id)
    {
        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleId == id);

        if (module == null)
        {
            return new ActionResponse<Module>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Module>
        {
            WasSuccess = true,
            Result = module
        };
    }

    public async Task<IEnumerable<Module>> GetComboAsync()
    {
        return await _context.Modules
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}