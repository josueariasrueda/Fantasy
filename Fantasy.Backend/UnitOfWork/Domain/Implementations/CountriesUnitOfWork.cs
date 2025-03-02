﻿using Fantasy.Backend.Repositories.Domain.Interfaces;
using Fantasy.Backend.UnitOfWork.Domain.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Implementatios;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitOfWork.Domain.Implementations;

public class CountriesUnitOfWork : GenericUnitOfWork<Country>, ICountriesUnitOfWork
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesUnitOfWork(IGenericRepository<Country> repository, ICountriesRepository countriesRepository) : base(repository)
    {
        _countriesRepository = countriesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync() => await _countriesRepository.GetAsync();

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination) => await _countriesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _countriesRepository.GetTotalRecordsAsync(pagination);

    public override async Task<ActionResponse<Country>> GetAsync(int id) => await _countriesRepository.GetAsync(id);

    public async Task<IEnumerable<Country>> GetComboAsync() => await _countriesRepository.GetComboAsync();
}