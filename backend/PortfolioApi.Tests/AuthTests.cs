using System.Net;

namespace PortfolioApi.Tests;

public class AuthTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();

    public void Dispose()
    {
        _factory.Dispose();
    }

    // Registreren

    [Fact]
    public async Task Register_GeldigeGegevens_Geeft200()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await TestHelpers.RegisterAsync(client, "nieuw@test.nl", TestHelpers.Wachtwoord);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tekst = await response.Content.ReadAsStringAsync();
        Assert.Contains("Account succesvol aangemaakt", tekst);
    }

    [Fact]
    public async Task Register_KrijgtRolGast()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        await TestHelpers.RegisterAsync(client, "nieuw@test.nl", TestHelpers.Wachtwoord);

        // Assert: de rol controleren we rechtstreeks bij Identity
        var rollen = await _factory.GetRolesAsync("nieuw@test.nl");
        Assert.Single(rollen);
        Assert.Contains("Gast", rollen);
    }

    [Fact]
    public async Task Register_DubbeleEmail_Geeft400()
    {
        // Arrange
        var client = _factory.MaakClient();
        await TestHelpers.RegisterAsync(client, "dubbel@test.nl", TestHelpers.Wachtwoord);

        // Act
        var response = await TestHelpers.RegisterAsync(client, "dubbel@test.nl", TestHelpers.Wachtwoord);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act: lang genoeg (8 tekens), maar geen hoofdletter en geen cijfer
        var response = await TestHelpers.RegisterAsync(client, "zwak@test.nl", "abcdefgh");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // login

    [Fact]
    public async Task Login_JuisteGegevens_Geeft200EnCookie()
    {
        // Arrange
        var client = _factory.MaakClient();
        await TestHelpers.RegisterAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);

        // Act
        var response = await TestHelpers.LoginAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);

        // Assert: statuscode en de cookie zit in de response
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Login_VerkeerdWachtwoord_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();
        await TestHelpers.RegisterAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);

        // Act
        var response = await TestHelpers.LoginAsync(client, "gast@test.nl", "Fout1234");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // me

    [Fact]
    public async Task Me_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_NaLogin_ToontEmailEnRol()
    {
        // Arrange
        var client = _factory.MaakClient();
        await TestHelpers.RegisterAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);
        await TestHelpers.LoginAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert: statuscode en inhoud
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tekst = await response.Content.ReadAsStringAsync();
        Assert.Contains("gast@test.nl", tekst);
        Assert.Contains("Gast", tekst);
    }

    // uitloggen
    [Fact]
    public async Task Logout_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsync("/api/auth/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_NaLogin_Geeft200EnMeDaarnaGeeft401()
    {
        // Arrange
        var client = _factory.MaakClient();
        await TestHelpers.RegisterAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);
        await TestHelpers.LoginAsync(client, "gast@test.nl", TestHelpers.Wachtwoord);

        // Act
        var response = await client.PostAsync("/api/auth/logout", null);

        // Assert: uitloggen lukt, en de cookie werkt niet meer
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var me = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
    }
}
