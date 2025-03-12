using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fantasy.Backend.Repositories.Infraestructure.Interfaces
{
    public interface IModulesRepository
    {
        Task<ActionResponse<IEnumerable<Module>>> GetAsync();

        Task<ActionResponse<Module>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<IEnumerable<Module>> GetComboAsync();
    }
}