using Fantasy.Backend.Repositories;
using Fantasy.Backend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork;

public interface ITenantsUnitOfWork
{
    Task<ActionResponse<Tenant>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Tenant>>> GetAsync();

    Task<ActionResponse<IEnumerable<Tenant>>> GetAsync(PaginationDTO pagination);

    Task<IEnumerable<Tenant>> GetComboAsync();

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}

public class TenantsUnitOfWork : GenericUnitOfWork<Tenant>, ITenantsUnitOfWork
{
    private readonly ITenantsRepository _tenantsRepository;

    public TenantsUnitOfWork(IGenericRepository<Tenant> repository, ITenantsRepository tenantsRepository) : base(repository)
    {
        _tenantsRepository = tenantsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Tenant>>> GetAsync() => await _tenantsRepository.GetAsync();

    public override async Task<ActionResponse<IEnumerable<Tenant>>> GetAsync(PaginationDTO pagination) => await _tenantsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _tenantsRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<Tenant>> GetAsync(int id) => await _tenantsRepository.GetAsync(id);

    public async Task<IEnumerable<Tenant>> GetComboAsync() => await _tenantsRepository.GetComboAsync();
}