using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Infraestructure.Interfaces;

public interface ITenantsRepository
{
    Task<ActionResponse<Tenant>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Tenant>>> GetAsync();

    Task<ActionResponse<IEnumerable<Tenant>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Tenant>> GetComboAsync();
}