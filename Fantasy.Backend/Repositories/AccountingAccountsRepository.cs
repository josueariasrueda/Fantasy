using Fantasy.Backend.Data;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories;

public interface IAccountingAccountsRepository
{
    Task<ActionResponse<AccountingAccount>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync();

    Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<AccountingAccount>> GetComboAsync();
}

public class AccountingAccountsRepository : GenericRepository<AccountingAccount>, IAccountingAccountsRepository
{
    private readonly AppDataContext _context;

    public AccountingAccountsRepository(AppDataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync()
    {
        var accounts = await _context.AccountingAccounts
            .OrderBy(a => a.Code)
            .ToListAsync();
        return new ActionResponse<IEnumerable<AccountingAccount>>
        {
            WasSuccess = true,
            Result = accounts
        };
    }

    public async Task<ActionResponse<IEnumerable<AccountingAccount>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.AccountingAccounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<AccountingAccount>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Code)
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)
                .Take(pagination.RecordsNumber)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.AccountingAccounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<AccountingAccount>> GetAsync(int id)
    {
        var account = await _context.AccountingAccounts.FindAsync(id);
        if (account == null)
        {
            return new ActionResponse<AccountingAccount>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<AccountingAccount>
        {
            WasSuccess = true,
            Result = account
        };
    }

    public async Task<IEnumerable<AccountingAccount>> GetComboAsync()
    {
        return await _context.AccountingAccounts
            .OrderBy(a => a.Code)
            .Select(a => new AccountingAccount
            {
                AccountingAccountId = a.AccountingAccountId,
                Code = a.Code,
                Name = a.Name
            })
            .ToListAsync();
    }
}