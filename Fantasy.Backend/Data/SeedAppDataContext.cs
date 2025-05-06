using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitOfWork;
using Fantasy.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Backend.Services;

using Fantasy.Backend.UnitOfWork;

namespace Fantasy.Backend.Data;

[ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
public class SeedAppDataContext
{
    private readonly AppDataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IUserService _userService;

    public SeedAppDataContext(AppDataContext context, IFileStorage fileStorage, IUsersUnitOfWork usersUnitOfWork, IUserService userService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _usersUnitOfWork = usersUnitOfWork;
        _userService = userService;
    }

    // AppDataContext
    // ======================
    // Crear el Tenant 0-Root
    // Crear Enteprprise 1-Default Enterprise

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        await CheckAppModulesAsync();       // Plannig, Control, Accounting
        await CheckAppRolesAsync();         // Admin, User
        await CheckAppCurrenciesAsync();    // Data\\Scripts\\Currencies.sql
        await CheckAppCountriesAsync();     // Data\\Scripts\\Countries.sql
        await CheckAppUsersAsync();         // bm.zulu@yopmail.com  123456
        await CheckAppTenantAsync();        // Root
        await CheckAppUsersTenantPermissionsAsync(); // Root
        await CheckAppEnterprisesAsync();   // Service Enterprise y la relacion EnterpriseTenant
        await CheckAppSubscriptionsAsync(); // Basic suscription, Premium subscription
    }

    private async Task CheckAppCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var SQLScript = File.ReadAllText("Data\\Scripts\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(SQLScript);
        }
    }

    private async Task CheckAppCurrenciesAsync()
    {
        if (!_context.Currencies.Any())
        {
            var SQLScript = File.ReadAllText("Data\\Scripts\\Currencies.sql");
            await _context.Database.ExecuteSqlRawAsync(SQLScript);
        }
    }

    private async Task CheckAppEnterprisesAsync()
    {
        // Verificar si ya existen empresas
        if (!_context.Enterprises.Any())
        {
            // Obtener el TenantId del tenant "Root"
            var rootTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == "Root");
            if (rootTenant == null)
            {
                throw new Exception("Root tenant not found. Please ensure the Root tenant is seeded first.");
            }

            // Obtener el usuario creador
            var appAdminUser = await _userService.GetFirstUserByTypeAsync(UserType.AppAdmin);
            if (appAdminUser == null)
            {
                throw new Exception("No AppAdmin user found. Please ensure at least one AppAdmin user exists.");
            }

            // Crear empresas por defecto
            var enterprises = new List<Enterprise>
                {
                    new Enterprise
                    {   Name = "Service Enterprise",
                        CreatedBy = appAdminUser.Id,
                        CreatedOn = DateTimeHelper.UtcNow()
                    },
                };

            // Agregar empresas a la base de datos
            _context.Enterprises.AddRange(enterprises);
            await _context.SaveChangesAsync();

            // Crear relaciones en la tabla EnterpriseTenant
            foreach (var enterprise in enterprises)
            {
                var currentUserId = await _userService.GetCurrentUserIdAsync();
                var enterpriseTenant = new EnterpriseTenant
                {
                    EnterpriseId = enterprise.EnterpriseId,
                    TenantId = rootTenant.TenantId
                };

                _context.Set<EnterpriseTenant>().Add(enterpriseTenant);
            }

            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppModulesAsync()
    {
        if (!_context.Modules.Any())
        {
            var modules = new List<Module>
        {
            new Module { Name = "Planning", Active = true }, // Planeación
            new Module { Name = "Control", Active = true },  // Control
            new Module { Name = "Accounting", Active = true } // Contabilización
        };

            _context.Modules.AddRange(modules);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.AppAdmin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.AppUser.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.AppUser.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task CheckAppSubscriptionsAsync()
    {
        if (!_context.Subscriptions.Any())
        {
            // Obtener el MultiTenant "Root"
            var rootTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == "Root");
            if (rootTenant == null)
            {
                throw new Exception("Root tenant not found. Please ensure the Root tenant is seeded first.");
            }

            // Obtener el usuario creador
            var appAdminUser = await _userService.GetFirstUserByTypeAsync(UserType.AppAdmin);
            if (appAdminUser == null)
            {
                throw new Exception("No AppAdmin user found. Please ensure at least one AppAdmin user exists.");
            }

            // Crear suscripciones iniciales
            var currentUserId = await _userService.GetCurrentUserIdAsync();
            var subscriptions = new List<Subscription>
        {
            new Subscription
            {
                Name = "Basic Subscription",
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                MaxUsers = 10,
                MaxEnterprises = 1,
                MaxElectronicsDocs = 1000,
                MaxSpace = 10, // 10 GB
                DiskSpace = 0, // Inicialmente sin uso
                Active = true,
                TenantId = rootTenant.TenantId,
                CreatedBy = appAdminUser.Id,
                CreatedOn = DateTimeHelper.UtcNow()
            }
        };

            // Agregar suscripciones a la base de datos
            _context.Subscriptions.AddRange(subscriptions);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppTenantAsync()
    {
        // Verificar si ya existe un tenant llamado "Root"
        if (!_context.Tenants.Any(t => t.Name == "Root"))
        {
            // Obtener el usuario creador
            var appAdminUser = await _userService.GetFirstUserByTypeAsync(UserType.AppAdmin);
            if (appAdminUser == null)
            {
                throw new Exception("No AppAdmin user found. Please ensure at least one AppAdmin user exists.");
            }

            var rootTenant = new Tenant
            {
                Code = "Root",
                Name = "Root",
                StoragePath = "Root",
                ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Fantasy-Root;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                CreatedBy = appAdminUser.Id,
                CreatedOn = DateTimeHelper.UtcNow()
            };

            // Agregar el tenant a la base de datos
            _context.Tenants.Add(rootTenant);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppUserTenantPermissionsAsync(User user, int tenantId)
    {
        var userTenantPermission = await _context.UsersTenantPermissions
            .FirstOrDefaultAsync(utp => utp.UserId == user.Id && utp.TenantId == tenantId);
        if (userTenantPermission == null)
        {
            userTenantPermission = new UserTenantPermission
            {
                UserId = user.Id,
                TenantId = tenantId,
                ModuleId = 1, // Cambia esto según el ID del módulo que desees asignar
                CanRead = true,
                CanWrite = true,
                CanEdit = true,
                CanDelete = true,
                OnlyMine = false
            };
            _context.UsersTenantPermissions.Add(userTenantPermission);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppUsersTenantPermissionsAsync()
    {
        // Buscar el tenant "Root"
        var rootTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == "Root");
        if (rootTenant == null)
        {
            throw new Exception("Root tenant not found. Please ensure the Root tenant is seeded first.");
        }

        // Obtener todos los usuarios de la base de datos
        var users = await _context.Users.ToListAsync();

        // Iterar sobre cada usuario y aplicar los permisos
        foreach (var user in users)
        {
            await CheckAppUserTenantPermissionsAsync(user, rootTenant.TenantId);
        }
    }

    private async Task CheckAppUsersAsync()
    {
        await CheckAppUserAsync("App", "admin", "bm.app.admin@yopmail.com", "322 311 4620", "JuanZuluaga.jpg", UserType.AppAdmin);
        await CheckAppUserAsync("App", "user", "bm.app.user@yopmail.com", "322 311 4620", "Brad.jpg", UserType.AppUser);
        await CheckAppUserAsync("Angelina", "Jolie", "bm.angelina@yopmail.com", "322 311 4620", "Angelina.jpg", UserType.User);
        await CheckAppUserAsync("Bob", "Marley", "bm.bob@yopmail.com", "322 311 4620", "bob.jpg", UserType.User);
        await CheckAppUserAsync("Celia", "Cruz", "bm.celia@yopmail.com", "322 311 4620", "celia.jpg", UserType.AppUser);
        await CheckAppUserAsync("Fredy", "Mercury", "bm.fredy@yopmail.com", "322 311 4620", "fredy.jpg", UserType.User);
        await CheckAppUserAsync("Hector", "Lavoe", "bm.hector@yopmail.com", "322 311 4620", "hector.jpg", UserType.User);
        await CheckAppUserAsync("Liv", "Taylor", "bm.liv@yopmail.com", "322 311 4620", "liv.jpg", UserType.User);
        await CheckAppUserAsync("Otep", "Shamaya", "bm.otep@yopmail.com", "322 311 4620", "otep.jpg", UserType.User);
        await CheckAppUserAsync("Ozzy", "Osbourne", "bm.ozzy@yopmail.com", "322 311 4620", "ozzy.jpg", UserType.User);
        await CheckAppUserAsync("Selena", "Quintanilla", "bm.selena@yopmail.com", "322 311 4620", "selena.jpg", UserType.User);
    }

    private async Task<User> CheckAppUserAsync(string firstName, string lastName, string email, string phone, string image, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{image}";
            //var fileBytes = File.ReadAllBytes(filePath);
            //var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");

            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Colombia");
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country = country!,
                UserType = userType,
                Photo = filePath
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456", filePath);
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }
}