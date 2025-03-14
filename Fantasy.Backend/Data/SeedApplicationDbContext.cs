﻿using Fantasy.Backend.Helpers;
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

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCurrenciesAsync();
        await CheckCountriesAsync();
        await CheckRolesAsync();
        await CheckUsersAsync();
    }

    private async Task CheckUsersAsync()
    {
        await CheckUserAsync("Juan", "Zuluaga", "bm.zulu@yopmail.com", "322 311 4620", "JuanZuluaga.jpg", UserType.Admin);
        await CheckUserAsync("Ledys", "Bedoya", "bm.ledys@yopmail.com", "322 311 4620", "LedysBedoya.jpg", UserType.User);
        await CheckUserAsync("Brad", "Pitt", "bm.brad@yopmail.com", "322 311 4620", "Brad.jpg", UserType.User);
        await CheckUserAsync("Angelina", "Jolie", "bm.angelina@yopmail.com", "322 311 4620", "Angelina.jpg", UserType.User);
        await CheckUserAsync("Bob", "Marley", "bm.bob@yopmail.com", "322 311 4620", "bob.jpg", UserType.User);
        await CheckUserAsync("Celia", "Cruz", "bm.celia@yopmail.com", "322 311 4620", "celia.jpg", UserType.Admin);
        await CheckUserAsync("Fredy", "Mercury", "bm.fredy@yopmail.com", "322 311 4620", "fredy.jpg", UserType.User);
        await CheckUserAsync("Hector", "Lavoe", "bm.hector@yopmail.com", "322 311 4620", "hector.jpg", UserType.User);
        await CheckUserAsync("Liv", "Taylor", "bm.liv@yopmail.com", "322 311 4620", "liv.jpg", UserType.User);
        await CheckUserAsync("Otep", "Shamaya", "bm.otep@yopmail.com", "322 311 4620", "otep.jpg", UserType.User);
        await CheckUserAsync("Ozzy", "Osbourne", "bm.ozzy@yopmail.com", "322 311 4620", "ozzy.jpg", UserType.User);
        await CheckUserAsync("Selena", "Quintanilla", "bm.selena@yopmail.com", "322 311 4620", "selena.jpg", UserType.User);
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, string image, UserType userType)
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

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var SQLScript = File.ReadAllText("Data\\Scripts\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(SQLScript);
        }
    }

    private async Task CheckCurrenciesAsync()
    {
        if (!_context.Currencies.Any())
        {
            var SQLScript = File.ReadAllText("Data\\Scripts\\Currencies.sql");
            await _context.Database.ExecuteSqlRawAsync(SQLScript);
        }
    }
}