﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitOfWork;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMailHelper _mailHelper;
    private readonly ApplicationDataContext _context;
    private readonly IFileStorage _fileStorage;

    public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration, IMailHelper mailHelper, ApplicationDataContext context, IFileStorage fileStorage)
    {
        _usersUnitOfWork = usersUnitOfWork;
        _configuration = configuration;
        _mailHelper = mailHelper;
        _context = context;
        _fileStorage = fileStorage;
    }

    [HttpOptions]
    public IActionResult HandlePreflight()
    {
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("paginated")]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _usersUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _usersUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpPost("RecoverPassword")]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var response = await SendRecoverEmailAsync(user, model.Language);
        if (response.WasSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors.FirstOrDefault()!.Description);
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        return NoContent();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> PutAsync(User user)
    {
        try
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Extraer el TenantId del contexto
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "TenantId");
            if (tenantIdClaim == null)
            {
                return BadRequest("TenantId is missing in the request.");
            }

            if (!int.TryParse(tenantIdClaim.Value, out var tenantId))
            {
                return BadRequest("Invalid TenantId format.");
            }

            // Validar que el usuario tiene acceso al TenantId
            if (!currentUser.UsersTenantPermissions.Any(utp => utp.TenantId == tenantId))
            {
                return BadRequest("User does not have access to the specified TenantId.");
            }

            // Procesar la foto del usuario
            if (!string.IsNullOrEmpty(user.Photo))
            {
                var photoUser = Convert.FromBase64String(user.Photo);
                user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", "users");
            }

            // Actualizar los datos del usuario
            currentUser.FirstName = user.FirstName;
            currentUser.LastName = user.LastName;
            currentUser.PhoneNumber = user.PhoneNumber;
            currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;

            var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
            if (result.Succeeded)
            {
                // Crear el token con el TenantId
                return Ok(BuildToken(currentUser, tenantId));
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
    }

    [HttpPost("ResedToken")]
    public async Task<IActionResult> ResedTokenAsync([FromBody] EmailDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var response = await SendConfirmationEmailAsync(user, model.Language);
        if (response.WasSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
    {
        var country = await _context.Countries.FindAsync(model.CountryId);
        if (country == null)
        {
            return BadRequest("ERR004");
        }

        User user = model;
        user.Country = country;
        var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _usersUnitOfWork.AddUserToRoleAsync(user, user.UserType.ToString());
            var response = await SendConfirmationEmailAsync(user, model.Language);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        token = token.Replace(" ", "+");
        var user = await _usersUnitOfWork.GetUserAsync(new Guid(userId));
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault());
        }

        return NoContent();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var result = await _usersUnitOfWork.LoginAsync(model);
        if (result.Succeeded)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);

            // Obtener los tenants asociados al usuario
            var tenants = user.UsersTenantPermissions.Select(utp => utp.Tenant).ToList();

            if (tenants.Count == 0)
            {
                // No tiene Tenants asociados
                return Ok(BuildToken(user, 0));
            }
            if (tenants.Count == 1)
            {
                // Si solo pertenece a un tenant, seleccionarlo automáticamente
                var selectedTenant = tenants.First();
                return Ok(BuildToken(user, selectedTenant.TenantId));
            }
            else if (tenants.Count > 1)
            {
                // Si pertenece a varios tenants, devolver la lista para que el frontend muestre una ventana modal
                return Ok(new
                {
                    RequiresTenantSelection = true,
                    Tenants = tenants.Select(t => new { t.TenantId, t.Name }) // Ajusta según las propiedades de MultiTenant
                });
            }

            return BadRequest("ERR_NO_TENANT");
        }

        if (result.IsLockedOut)
        {
            return BadRequest("ERR007");
        }

        if (result.IsNotAllowed)
        {
            return BadRequest("ERR008");
        }

        return BadRequest("ERR006");
    }

    private TokenDTO BuildToken(User user, int tenantId)
    {
        var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Email!),
        new(ClaimTypes.Role, user.UserType.ToString()),
        new("FirstName", user.FirstName),
        new("LastName", user.LastName),
        new("UserName", $"{user.FirstName} {user.LastName}"),
        new("Photo", user.Photo ?? string.Empty),
        new("TenantId", tenantId.ToString()) // Incluir el TenantId en el token
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }

    private async Task<ActionResponse<string>> SendRecoverEmailAsync(User user, string language)
    {
        var myToken = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);
        var tokenLink = Url.Action("ResetPassword", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        if (language == "es")
        {
            return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEs"]!, string.Format(_configuration["Mail:BodyRecoveryEs"]!, tokenLink), language);
        }
        return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEn"]!, string.Format(_configuration["Mail:BodyRecoveryEn"]!, tokenLink), language);
    }

    private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user, string language)
    {
        var myToken = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
        var tokenLink = Url.Action("ConfirmEmail", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        if (language == "es")
        {
            return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectConfirmationEs"]!, string.Format(_configuration["Mail:BodyConfirmationEs"]!, tokenLink), language);
        }
        return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectConfirmationEn"]!, string.Format(_configuration["Mail:BodyConfirmationEn"]!, tokenLink), language);
    }
}