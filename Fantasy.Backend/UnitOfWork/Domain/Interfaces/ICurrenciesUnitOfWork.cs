using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICurrenciesUnitOfWork
{
    Task<ActionResponse<Currency>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Currency>>> GetAsync();
    Task<ActionResponse<IEnumerable<Currency>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Currency>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}


