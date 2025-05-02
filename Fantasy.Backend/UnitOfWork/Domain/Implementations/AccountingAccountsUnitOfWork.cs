using Fantasy.Backend.Repositories.Domain.Interfaces;
using Fantasy.Backend.UnitOfWork.Domain.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork.Domain.Implementations
{
    public class AccountingAccountsUnitOfWork : IAccountingAccountsUnitOfWork
    {
        private readonly IAccountingAccountsRepository _accountingAccountsRepository;

        public AccountingAccountsUnitOfWork(IAccountingAccountsRepository accountingAccountsRepository)
        {
            _accountingAccountsRepository = accountingAccountsRepository;
        }

        public async Task<ActionResponse<AccountingAccount>> GetAsync(int id)
        {
            return await _accountingAccountsRepository.GetAsync(id);
        }

        public async Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync()
        {
            return await _accountingAccountsRepository.GetAsync();
        }

        public async Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync(PaginationDTO pagination)
        {
            return await _accountingAccountsRepository.GetAsync(pagination);
        }

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            return await _accountingAccountsRepository.GetTotalRecordsAsync(pagination);
        }

        public async Task<IEnumerable<AccountingAccount>> GetComboAsync()
        {
            return await _accountingAccountsRepository.GetComboAsync();
        }
    }
}