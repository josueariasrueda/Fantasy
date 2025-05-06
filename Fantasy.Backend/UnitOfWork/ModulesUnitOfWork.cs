using Fantasy.Backend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork;

public interface IModulesUnitOfWork
{
    Task<ActionResponse<Module>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Module>>> GetAsync();

    Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination);

    Task<IEnumerable<Module>> GetComboAsync();

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}

public class ModulesUnitOfWork : IModulesUnitOfWork
{
    private readonly IModulesRepository _modulesRepository;

    public ModulesUnitOfWork(IModulesRepository modulesRepository)
    {
        _modulesRepository = modulesRepository;
    }

    public async Task<ActionResponse<IEnumerable<Module>>> GetAsync() => await _modulesRepository.GetAsync();

    public async Task<ActionResponse<IEnumerable<Module>>> GetAsync(PaginationDTO pagination) => await _modulesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _modulesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Module>> GetAsync(int id) => await _modulesRepository.GetAsync(id);

    public async Task<IEnumerable<Module>> GetComboAsync() => await _modulesRepository.GetComboAsync();
}