using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Data;

namespace PortfolioApi_Test;

public abstract class IntegrationTestBase : IClassFixture<PortfolioApiFactory>
{
    protected readonly PortfolioApiFactory Factory;

    protected IntegrationTestBase(PortfolioApiFactory factory)
    {
        Factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        db.Database.EnsureCreated();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Gast", "Editor", "Admin" })
        {
            if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }

    /// <summary>Anonieme client: 401 i.p.v. 302 redirect naar login.</summary>
    protected HttpClient CreateAnonymousClient() =>
        Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    /// <summary>Maakt een gebruiker met rol aan en logt in; cookie zit in de handler.</summary>
    protected async Task<HttpClient> CreateLoggedInClientAsync(
        string email, string password, string role)
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser { UserName = email, Email = email };
            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", created.Errors.Select(e => e.Description)));
            await userManager.AddToRoleAsync(user, role);
        }

        var client = CreateAnonymousClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        if (login.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Login faalde met status {(int)login.StatusCode} voor {email}");
        return client;
    }

    protected static string UniqueEmail(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}@test.nl";

    protected const string ValidPassword = "Wachtwoord1!";
}
