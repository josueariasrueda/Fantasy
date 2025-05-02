using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork.Domain.Interfaces;

public interface IAccountingAccountsUnitOfWork
{
    Task<ActionResponse<AccountingAccount>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync();

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<AccountingAccount>> GetComboAsync();
}