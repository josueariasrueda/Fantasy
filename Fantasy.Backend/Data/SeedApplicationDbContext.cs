using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitsOfWork.Implementations;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

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

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckRolesAsync();
        await CheckUsersAsync();
    }

    private async Task CheckUsersAsync()
    {
        await CheckUserAsync("Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "JuanZuluaga.jpg", UserType.Admin);
        await CheckUserAsync("Ledys", "Bedoya", "ledys@yopmail.com", "322 311 4620", "LedysBedoya.jpg", UserType.User);
        await CheckUserAsync("Brad", "Pitt", "brad@yopmail.com", "322 311 4620", "Brad.jpg", UserType.User);
        await CheckUserAsync("Angelina", "Jolie", "angelina@yopmail.com", "322 311 4620", "Angelina.jpg", UserType.User);
        await CheckUserAsync("Bob", "Marley", "bob@yopmail.com", "322 311 4620", "bob.jpg", UserType.User);
        await CheckUserAsync("Celia", "Cruz", "celia@yopmail.com", "322 311 4620", "celia.jpg", UserType.Admin);
        await CheckUserAsync("Fredy", "Mercury", "fredy@yopmail.com", "322 311 4620", "fredy.jpg", UserType.User);
        await CheckUserAsync("Hector", "Lavoe", "hector@yopmail.com", "322 311 4620", "hector.jpg", UserType.User);
        await CheckUserAsync("Liv", "Taylor", "liv@yopmail.com", "322 311 4620", "liv.jpg", UserType.User);
        await CheckUserAsync("Otep", "Shamaya", "otep@yopmail.com", "322 311 4620", "otep.jpg", UserType.User);
        await CheckUserAsync("Ozzy", "Osbourne", "ozzy@yopmail.com", "322 311 4620", "ozzy.jpg", UserType.User);
        await CheckUserAsync("Selena", "Quintanilla", "selena@yopmail.com", "322 311 4620", "selena.jpg", UserType.User);
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, string image, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{image}";
            var fileBytes = File.ReadAllBytes(filePath);
            var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");

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
                Photo = imagePath
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesStatesCitiesSQLScript = File.ReadAllText("Data\\Scripts\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesStatesCitiesSQLScript);
        }
    }
}