﻿using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories;

public interface IEnterprisesRepository
{
    Task<ActionResponse<Enterprise>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync();

    Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Enterprise>> GetComboAsync();
}

public class EnterprisesRepository : GenericRepository<Enterprise>, IEnterprisesRepository
{
    private readonly AppDataContext _context;

    public EnterprisesRepository(AppDataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync()
    {
        var enterprises = await _context.Enterprises
            .OrderBy(e => e.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Enterprise>>
        {
            WasSuccess = true,
            Result = enterprises
        };
    }

    public override async Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Enterprises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Enterprise>>
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
        var queryable = _context.Enterprises.AsQueryable();

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

    public override async Task<ActionResponse<Enterprise>> GetAsync(int id)
    {
        var enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(e => e.EnterpriseId == id);

        if (enterprise == null)
        {
            return new ActionResponse<Enterprise>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Enterprise>
        {
            WasSuccess = true,
            Result = enterprise
        };
    }

    public async Task<IEnumerable<Enterprise>> GetComboAsync()
    {
        return await _context.Enterprises
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}