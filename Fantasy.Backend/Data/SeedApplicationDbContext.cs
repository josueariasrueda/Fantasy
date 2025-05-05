using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitOfWork.Infraestructure.Interfaces;
using Fantasy.Backend.UnitOfWork.Infraestructure.Implementatios;
using Fantasy.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Fantasy.Shared.Entities.Infraestructure;

namespace Fantasy.Backend.Data;

[ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
public class SeedApplicationDbContext
{
    private readonly ApplicationDataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedApplicationDbContext(ApplicationDataContext context, IFileStorage fileStorage, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _fileStorage = fileStorage;
        _usersUnitOfWork = usersUnitOfWork;
    }

    // ApplicationDataContext
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
        await CheckAppTenantAsync();        // Root
        await CheckAppUsersAsync();         // bm.zulu@yopmail.com  123456
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

            // Crear empresas por defecto
            var enterprises = new List<Enterprise>
        {
            new Enterprise { Name = "Service Enterprise" },
        };

            // Agregar empresas a la base de datos
            _context.Enterprises.AddRange(enterprises);
            await _context.SaveChangesAsync();

            // Crear relaciones en la tabla EnterpriseTenant
            foreach (var enterprise in enterprises)
            {
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
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
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

            // Crear suscripciones iniciales
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
                TenantId = rootTenant.TenantId
            },
            new Subscription
            {
                Name = "Premium Subscription",
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                MaxUsers = 50,
                MaxEnterprises = 5,
                MaxElectronicsDocs = 10000,
                MaxSpace = 50, // 50 GB
                DiskSpace = 0,
                Active = true,
                TenantId = rootTenant.TenantId
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
            // Crear el tenant por defecto
            var rootTenant = new Tenant
            {
                Code = "Root",
                Name = "Root",
                StoragePath = "Root",
                ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Fantasy-Root;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                IsActive = true
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
                ModuleId = 1 // Cambia esto según el ID del módulo que desees asignar
            };
            _context.UsersTenantPermissions.Add(userTenantPermission);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckAppUsersAsync()
    {
        // Verificar si ya existe un tenant llamado "Root"
        if (_context.Tenants.Any(t => t.Name == "Root"))
        {
            var rootTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == "Root");
            await CheckAppUserAsync("Juan", "Zuluaga", "bm.zulu@yopmail.com", "322 311 4620", "JuanZuluaga.jpg", UserType.Admin, rootTenant.TenantId);
            await CheckAppUserAsync("Ledys", "Bedoya", "bm.ledys@yopmail.com", "322 311 4620", "LedysBedoya.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Brad", "Pitt", "bm.brad@yopmail.com", "322 311 4620", "Brad.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Angelina", "Jolie", "bm.angelina@yopmail.com", "322 311 4620", "Angelina.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Bob", "Marley", "bm.bob@yopmail.com", "322 311 4620", "bob.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Celia", "Cruz", "bm.celia@yopmail.com", "322 311 4620", "celia.jpg", UserType.Admin, rootTenant.TenantId);
            await CheckAppUserAsync("Fredy", "Mercury", "bm.fredy@yopmail.com", "322 311 4620", "fredy.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Hector", "Lavoe", "bm.hector@yopmail.com", "322 311 4620", "hector.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Liv", "Taylor", "bm.liv@yopmail.com", "322 311 4620", "liv.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Otep", "Shamaya", "bm.otep@yopmail.com", "322 311 4620", "otep.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Ozzy", "Osbourne", "bm.ozzy@yopmail.com", "322 311 4620", "ozzy.jpg", UserType.User, rootTenant.TenantId);
            await CheckAppUserAsync("Selena", "Quintanilla", "bm.selena@yopmail.com", "322 311 4620", "selena.jpg", UserType.User, rootTenant.TenantId);
        }
    }

    private async Task<User> CheckAppUserAsync(string firstName, string lastName, string email, string phone, string image, UserType userType, int tenantId)
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
            await CheckAppUserTenantPermissionsAsync(user, tenantId);
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }
}