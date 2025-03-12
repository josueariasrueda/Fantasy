using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;
using Fantasy.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICurrenciesRepository
{
    Task<ActionResponse<Currency>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Currency>>> GetAsync();

    Task<ActionResponse<IEnumerable<Currency>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Currency>> GetComboAsync();
}