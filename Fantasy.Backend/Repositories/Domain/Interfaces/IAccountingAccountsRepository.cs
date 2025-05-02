using Fantasy.Shared.DTOs;
using Fantasy.Shared.Responses;
using Fantasy.Shared.Entities.Domain;

namespace Fantasy.Backend.Repositories.Domain.Interfaces;

public interface IAccountingAccountsRepository
{
    Task<ActionResponse<AccountingAccount>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync();

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<AccountingAccount>> GetComboAsync();
}