﻿using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Domain.Interfaces;

public interface ICountriesRepository
{
    Task<ActionResponse<Country>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Country>>> GetAsync();

    Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<IEnumerable<Country>> GetComboAsync();
}