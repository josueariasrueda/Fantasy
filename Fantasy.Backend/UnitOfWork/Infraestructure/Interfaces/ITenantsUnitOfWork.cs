using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces
{
    public interface ITenantsUnitOfWork
    {
        Task<ActionResponse<Tenant>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Tenant>>> GetAsync();

        Task<ActionResponse<IEnumerable<Tenant>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Tenant>> GetComboAsync();

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}