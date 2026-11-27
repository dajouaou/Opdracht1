using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Data;

namespace PortfolioApi.Tests;

// start de echte API in het geheugen maar met een eigen SQLite-database so that test nooit de SQL Server database PortfolioOpdracht3 aanraakt
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Echte database (SQL Server) eruit
            var teVerwijderen = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<PortfolioDbContext>)
                         || (d.ServiceType.IsGenericType
                             && d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration")
                             && d.ServiceType.GenericTypeArguments[0] == typeof(PortfolioDbContext)))
                .ToList();
            foreach (var d in teVerwijderen)
            {
                services.Remove(d);
            }

            //  SQLite in-memory erin. De verbinding blijft open zolang de factory leeft anders is de database na elk verzoek weg.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            services.AddDbContext<PortfolioDbContext>(opties => opties.UseSqlite(_connection));

            // tabellen aanmaken en de rollen Gast, Editor en Admin klaarzetten
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
            db.Database.EnsureCreated();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            RolSeeder.SeedRolesAsync(roleManager).GetAwaiter().GetResult();
        });
    }

    // een client die cookies onthoudt (nodig voor inloggen).
    public HttpClient MaakClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
    }

    // maakt een gebruiker aan met een rol, before het inloggen.
    // nodig voor Editor en Admin, want registratie geeft alleen Gast.
    public async Task CreateUserWithRoleAsync(string email, string password, string role)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var gebruiker = new IdentityUser { UserName = email, Email = email };
        var resultaat = await userManager.CreateAsync(gebruiker, password);
        if (!resultaat.Succeeded)
        {
            throw new Exception("Gebruiker aanmaken mislukt: " +
                string.Join(", ", resultaat.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(gebruiker, role);
    }

    // geeft de rollen van een gebruiker, om te controleren wat registratie heeft gedaan.
    public async Task<IList<string>> GetRolesAsync(string email)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var gebruiker = await userManager.FindByEmailAsync(email);
        return await userManager.GetRolesAsync(gebruiker!);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
