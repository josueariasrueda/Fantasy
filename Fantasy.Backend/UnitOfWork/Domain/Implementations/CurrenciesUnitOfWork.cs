using Fantasy.Backend.Repositories.Domain.Interfaces;
using Fantasy.Backend.UnitOfWork.Domain.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Implementatios;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CurrenciesUnitOfWork : GenericUnitOfWork<Currency>, ICurrenciesUnitOfWork
{
    private readonly ICurrenciesRepository _currenciesRepository;

    public CurrenciesUnitOfWork(IGenericRepository<Currency> repository, ICurrenciesRepository currenciesRepository) : base(repository)
    {
        _currenciesRepository = currenciesRepository;
    }

    public async Task<ActionResponse<IEnumerable<Currency>>> GetAsync() => await _currenciesRepository.GetAsync();

    public async Task<ActionResponse<IEnumerable<Currency>>> GetAsync(PaginationDTO pagination) => await _currenciesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _currenciesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Currency>> GetAsync(int id) => await _currenciesRepository.GetAsync(id);

    public async Task<IEnumerable<Currency>> GetComboAsync() => await _currenciesRepository.GetComboAsync();
}