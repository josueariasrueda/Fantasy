using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces
{
    public interface IModulesUnitOfWork
    {
        Task<ActionResponse<Module>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Module>>> GetAsync();

        Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Module>> GetComboAsync();

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}