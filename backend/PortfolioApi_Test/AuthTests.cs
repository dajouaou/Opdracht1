using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi_Test;

public class AuthTests(PortfolioApiFactory factory) : IntegrationTestBase(factory)
{
    private record MeResponse(string Email, List<string> Roles);

    [Fact]
    public async Task Register_NewUser_ReturnsOkAndAssignsGastRole()
    {
        // Arrange
        var client = CreateAnonymousClient();
        var email = UniqueEmail("nieuw");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email, password = ValidPassword });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user!, "Gast"));
        Assert.False(await userManager.IsInRoleAsync(user!, "Editor"));
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateAnonymousClient();
        var email = UniqueEmail("dubbel");
        await client.PostAsJsonAsync("/api/auth/register", new { email, password = ValidPassword });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email, password = ValidPassword });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email = UniqueEmail("zwak"), password = "123" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkAndCookieWorksOnMe()
    {
        // Arrange
        var email = UniqueEmail("login");
        var client = await CreateLoggedInClientAsync(email, ValidPassword, "Gast");

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<MeResponse>();
        Assert.NotNull(body);
        Assert.Equal(email, body!.Email);
        Assert.Contains("Gast", body.Roles);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var email = UniqueEmail("fout");
        await CreateLoggedInClientAsync(email, ValidPassword, "Gast");
        var client = CreateAnonymousClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "FoutWachtwoord1!" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = UniqueEmail("bestaatniet"), password = ValidPassword });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithoutLogin_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutLogin_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.PostAsync("/api/auth/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_AfterLogin_ThenMeReturnsUnauthorized()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("logout"), ValidPassword, "Gast");

        // Act
        var logout = await client.PostAsync("/api/auth/logout", null);
        var me = await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
    }

    [Fact]
    public async Task RoleChange_OnlyAppliesAfterReLogin()
    {
        // Arrange: Gast logt in, krijgt daarna Editor-rol in de database
        var email = UniqueEmail("promote");
        var client = await CreateLoggedInClientAsync(email, ValidPassword, "Gast");

        using (var scope = Factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, "Editor");
        }

        // Act 1: oude cookie bevat nog de oude rolclaims
        var beforeRelogin = await client.PostAsJsonAsync("/api/projects", TestData.Project());

        // Assert 1: nog steeds 403
        Assert.Equal(HttpStatusCode.Forbidden, beforeRelogin.StatusCode);

        // Act 2: opnieuw inloggen
        var relogin = await client.PostAsJsonAsync("/api/auth/login",
            new { email, password = ValidPassword });
        var afterRelogin = await client.PostAsJsonAsync("/api/projects", TestData.Project());

        // Assert 2: nu 201
        Assert.Equal(HttpStatusCode.OK, relogin.StatusCode);
        Assert.Equal(HttpStatusCode.Created, afterRelogin.StatusCode);
    }
}
