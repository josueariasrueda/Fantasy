using Fantasy.Backend.Data;
using Fantasy.Backend.Repositories.Infraestructure.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories.Infraestructure.Implementations
{
    public class SubscriptionsRepository : ISubscriptionsRepository
    {
        private readonly ApplicationDataContext _context;

        public SubscriptionsRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<IEnumerable<Subscription>>> GetAsync()
        {
            var subscriptions = await _context.Subscriptions
                .OrderBy(s => s.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<Subscription>>
            {
                WasSuccess = true,
                Result = subscriptions
            };
        }

        public async Task<ActionResponse<IEnumerable<Subscription>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Subscriptions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Subscription>>
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
            var queryable = _context.Subscriptions.AsQueryable();

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

        public async Task<ActionResponse<Subscription>> GetAsync(int id)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriptionId == id);

            if (subscription == null)
            {
                return new ActionResponse<Subscription>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Subscription>
            {
                WasSuccess = true,
                Result = subscription
            };
        }

        public async Task<IEnumerable<Subscription>> GetComboAsync()
        {
            return await _context.Subscriptions
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}