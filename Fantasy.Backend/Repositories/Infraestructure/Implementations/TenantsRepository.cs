using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Infraestructure.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories.Infraestructure.Implementations
{
    public class TenantsRepository : ITenantsRepository
    {
        private readonly ApplicationDataContext _context;

        public TenantsRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<IEnumerable<Tenant>>> GetAsync()
        {
            var tenants = await _context.Tenants
                .OrderBy(t => t.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<Tenant>>
            {
                WasSuccess = true,
                Result = tenants
            };
        }

        public async Task<ActionResponse<IEnumerable<Tenant>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Tenants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Tenant>>
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
            var queryable = _context.Tenants.AsQueryable();

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

        public async Task<ActionResponse<Tenant>> GetAsync(int id)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
            {
                return new ActionResponse<Tenant>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Tenant>
            {
                WasSuccess = true,
                Result = tenant
            };
        }

        public async Task<IEnumerable<Tenant>> GetComboAsync()
        {
            return await _context.Tenants
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}