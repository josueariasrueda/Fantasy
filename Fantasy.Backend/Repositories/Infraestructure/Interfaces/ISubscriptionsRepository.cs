using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISubscriptionsRepository
{
    Task<ActionResponse<Subscription>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync();
    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<IEnumerable<Subscription>> GetComboAsync();
}