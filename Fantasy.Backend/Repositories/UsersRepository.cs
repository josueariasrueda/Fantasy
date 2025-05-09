﻿using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Enums;
using Fantasy.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories;

public interface IUsersRepository
{
    Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<string> GeneratePasswordResetTokenAsync(User user);

    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    Task<IdentityResult> UpdateUserAsync(User user);

    Task<User?> GetFirstUserByTypeAsync(UserType userType);

    Task<User> GetUserAsync(string email);

    Task<User> GetUserAsync(Guid userId);

    Task<string> GenerateEmailConfirmationTokenAsync(User user);

    Task<IdentityResult> ConfirmEmailAsync(User user, string token);

    Task<IdentityResult> AddUserAsync(User user, string password);

    Task<IdentityResult> AddUserAsync(User user, string password, string localpathphoto);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);

    Task<SignInResult> LoginAsync(LoginDTO model);

    Task LogoutAsync();
}

public class UsersRepository : IUsersRepository
{
    private readonly AppDataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IFileStorage _fileStorage;
    private readonly IFileService _fileService;

    public UsersRepository(AppDataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IFileStorage fileStorage, IFileService fileService)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _fileStorage = fileStorage;
        _fileService = fileService;
    }

    public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Users
            .Include(x => x.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                x.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<User>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                x.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
    {
        return await _userManager.ResetPasswordAsync(user, token, password);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<User?> GetFirstUserByTypeAsync(UserType userType)
    {
        return await _context.Users
            .Where(u => u.UserType == userType)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Country)
            .FirstOrDefaultAsync(x => x.Id == userId.ToString());
        return user!;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<SignInResult> LoginAsync(LoginDTO model)
    {
        return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password, string localpathphoto)
    {
        if (!string.IsNullOrEmpty(localpathphoto) && !localpathphoto.StartsWith("http"))
        {
            user.Photo = await _fileService.SaveUserPhotoAsync(localpathphoto);
        }

        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task CheckRoleAsync(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });
        }
    }

    public async Task<User> GetUserAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.UsersTenantPermissions)
            .ThenInclude(utp => utp.Tenant)
            .FirstOrDefaultAsync(u => u.Email == email);

        Console.WriteLine($"User: {user?.Email}, TenantPermissions: {user?.UsersTenantPermissions.Count}");
        foreach (var permission in user?.UsersTenantPermissions ?? Enumerable.Empty<UserTenantPermission>())
        {
            Console.WriteLine($"TenantId: {permission.TenantId}, TenantName: {permission.Tenant?.Name}");
        }

        return user!;
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }
}