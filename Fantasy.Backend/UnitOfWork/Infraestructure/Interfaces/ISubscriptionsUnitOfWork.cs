using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISubscriptionsUnitOfWork
{
    Task<ActionResponse<Subscription>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync();
    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Subscription>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}