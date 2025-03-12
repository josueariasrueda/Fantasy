using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces
{
    public interface IEnterprisesUnitOfWork
    {
        Task<ActionResponse<Enterprise>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync();

        Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Enterprise>> GetComboAsync();

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}