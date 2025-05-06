using Fantasy.Backend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork;

public interface ISubscriptionsUnitOfWork
{
    Task<ActionResponse<Subscription>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync();

    Task<ActionResponse<IEnumerable<Subscription>>> GetAsync(PaginationDTO pagination);

    Task<IEnumerable<Subscription>> GetComboAsync();

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}

public class SubscriptionsUnitOfWork : GenericUnitOfWork<Subscription>, ISubscriptionsUnitOfWork
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public SubscriptionsUnitOfWork(IGenericRepository<Subscription> repository, ISubscriptionsRepository subscriptionsRepository) : base(repository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Subscription>>> GetAsync() => await _subscriptionsRepository.GetAsync();

    public override async Task<ActionResponse<IEnumerable<Subscription>>> GetAsync(PaginationDTO pagination) => await _subscriptionsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _subscriptionsRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<Subscription>> GetAsync(int id) => await _subscriptionsRepository.GetAsync(id);

    public async Task<IEnumerable<Subscription>> GetComboAsync() => await _subscriptionsRepository.GetComboAsync();
}