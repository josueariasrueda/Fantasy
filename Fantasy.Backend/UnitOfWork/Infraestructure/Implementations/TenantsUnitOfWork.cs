using Fantasy.Backend.Repositories.Infraestructure.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Implementations;
using Fantasy.Backend.UnitOfWork.Infraestructure.Implementatios;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fantasy.Backend.Repositories.Domain.Interfaces;

namespace Fantasy.Backend.UnitOfWork.Infraestructure.Implementations
{
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
}