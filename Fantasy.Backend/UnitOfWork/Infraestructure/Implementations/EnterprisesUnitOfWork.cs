using Fantasy.Backend.Repositories.Infraestructure.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fantasy.Backend.UnitOfWork.Infraestructure.Implementations
{
    public class EnterprisesUnitOfWork : IEnterprisesUnitOfWork
    {
        private readonly IEnterprisesRepository _enterprisesRepository;

        public EnterprisesUnitOfWork(IEnterprisesRepository enterprisesRepository)
        {
            _enterprisesRepository = enterprisesRepository;
        }

        public async Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync() => await _enterprisesRepository.GetAsync();

        public async Task<ActionResponse<IEnumerable<Enterprise>>> GetAsync(PaginationDTO pagination) => await _enterprisesRepository.GetAsync(pagination);

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _enterprisesRepository.GetTotalRecordsAsync(pagination);

        public async Task<ActionResponse<Enterprise>> GetAsync(int id) => await _enterprisesRepository.GetAsync(id);

        public async Task<IEnumerable<Enterprise>> GetComboAsync() => await _enterprisesRepository.GetComboAsync();
    }
}