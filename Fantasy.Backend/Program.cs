using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.MultiTenant;
using Fantasy.Backend.Repositories;
using Fantasy.Backend.Services;
using Fantasy.Backend.UnitOfWork;
using Fantasy.Shared.Entities.Infraestructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

[ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(c =>
        {   // https://localhost:7230/swagger/index.html
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
              {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
                });
        });

        // Inyeccion de dependencia para la base de datos y MultiTenant
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        builder.Services.AddDbContext<ApplicationDataContext>(x => x.UseSqlServer("name=ApplicationDataConnection"));
        builder.Services.AddTransient<TenantDbContextFactory>();
        builder.Services.AddTransient<ITenantService, TenantService>();
        builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();
        builder.Services.AddTransient<SeedApplicationDbContext>();

        // Agregar configuración desde appsettings.json
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IFileStorage, FileStorage>();
        builder.Services.AddScoped<IMailHelper, MailHelper>();
        builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();

        builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddScoped<IAccountingAccountsRepository, AccountingAccountsRepository>();
        builder.Services.AddScoped<IAccountingAccountsUnitOfWork, AccountingAccountsUnitOfWork>();
        builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
        builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();
        builder.Services.AddScoped<ICurrenciesRepository, CurrenciesRepository>();
        builder.Services.AddScoped<ICurrenciesUnitOfWork, CurrenciesUnitOfWork>();
        builder.Services.AddScoped<IEnterprisesRepository, EnterprisesRepository>();
        builder.Services.AddScoped<IEnterprisesUnitOfWork, EnterprisesUnitOfWork>();
        builder.Services.AddScoped<IModulesRepository, ModulesRepository>();
        builder.Services.AddScoped<IModulesUnitOfWork, ModulesUnitOfWork>();
        builder.Services.AddScoped<ISubscriptionsUnitOfWork, SubscriptionsUnitOfWork>();
        builder.Services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        builder.Services.AddScoped<ITenantsRepository, TenantsRepository>();
        builder.Services.AddScoped<ITenantsUnitOfWork, TenantsUnitOfWork>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

        builder.Services.AddIdentity<User, IdentityRole>(x =>
        {
            x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            x.SignIn.RequireConfirmedEmail = true;
            x.User.RequireUniqueEmail = true;
            x.Password.RequireDigit = false;
            x.Password.RequiredUniqueChars = 0;
            x.Password.RequireLowercase = false;
            x.Password.RequireNonAlphanumeric = false;
            x.Password.RequireUppercase = false;
            x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            x.Lockout.MaxFailedAccessAttempts = 3;
            x.Lockout.AllowedForNewUsers = true;
        })
            .AddEntityFrameworkStores<ApplicationDataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
                ClockSkew = TimeSpan.Zero
            });

        var app = builder.Build();

        SeedData(app);
        void SeedData(WebApplication app)
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory!.CreateScope();
            var service = scope.ServiceProvider.GetService<SeedApplicationDbContext>();
            service!.SeedAsync().Wait();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Paso 3: Agregando y habilitando el middleware para generar la documentación de Swagger
            app.UseSwagger();      // Generate Swagger JSON
            app.UseSwaggerUI();    // Serve Swagger UI for interactive API documentation
        }

        app.UseStaticFiles(); // Para servir archivos estáticos
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

        app.UseMiddleware<TenantMiddleware>(); // Middleware de tenant
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}