using System.Net.Http.Json;
using PortfolioApi.DTOs;

namespace PortfolioApi.Tests;

public static class TestHelpers
{
    // voldoet aan minstens 8 tekens, hoofdletter, kleine letter en cijfer
    public const string Wachtwoord = "Test1234";

    // POST /api/auth/register
    public static async Task<HttpResponseMessage> RegisterAsync(HttpClient client, string email, string password)
    {
        var dto = new RegisterDto { Email = email, Password = password };
        return await client.PostAsJsonAsync("/api/auth/register", dto);
    }

    // POST /api/auth/login. De cookie blijft daarna op deze HttpClient staan.
    public static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string email, string password)
    {
        var dto = new LoginDto { Email = email, Password = password };
        return await client.PostAsJsonAsync("/api/auth/login", dto);
    }

    // maakt een gebruiker met de gegeven rol, logt in en geeft de ingelogde client terug.
    public static async Task<HttpClient> MaakIngelogdeClientAsync(CustomWebApplicationFactory factory, string rol)
    {
        var email = rol.ToLower() + "@test.nl";
        await factory.CreateUserWithRoleAsync(email, Wachtwoord, rol);

        var client = factory.MaakClient();
        var response = await LoginAsync(client, email, Wachtwoord);
        Assert.True(response.IsSuccessStatusCode, "Inloggen in de testopzet is mislukt");
        return client;
    }
}
