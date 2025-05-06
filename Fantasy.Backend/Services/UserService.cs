using Fantasy.Backend.Repositories;
using Fantasy.Backend.UnitOfWork;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fantasy.Backend.Services;

public interface IUserService
{
    Task<ClaimsPrincipal> GetClaimsPrincipalAsync();

    Task<User?> GetCurrentUserAsync();

    Task<string?> GetCurrentUserEmailAsync();

    Task<bool> IsUserAuthenticatedAsync();

    Task<string?> GetCurrentUserIdAsync();

    Task<User?> GetFirstUserByTypeAsync(UserType userType);
}

public class UserService : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(AuthenticationStateProvider authenticationStateProvider, IUsersUnitOfWork usersUnitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _usersUnitOfWork = usersUnitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var claimsPrincipal = await GetClaimsPrincipalAsync();

        if (!claimsPrincipal.Identity?.IsAuthenticated ?? true)
        {
            return null; // Usuario no autenticado
        }

        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return null; // No se encontró el correo en los claims
        }

        // Obtener el usuario del repositorio
        return await _usersUnitOfWork.GetUserAsync(email);
    }

    public async Task<string?> GetCurrentUserEmailAsync()
    {
        var user = await GetClaimsPrincipalAsync();
        return user.Identity?.IsAuthenticated == true
            ? user.FindFirst(ClaimTypes.Email)?.Value
            : null;
    }

    public async Task<string?> GetCurrentUserIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return await Task.FromResult(user?.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        var user = await GetClaimsPrincipalAsync();
        return user.Identity?.IsAuthenticated == true;
    }

    public async Task<User?> GetFirstUserByTypeAsync(UserType userType)
    {
        return await _usersUnitOfWork.GetFirstUserByTypeAsync(userType);
    }
}