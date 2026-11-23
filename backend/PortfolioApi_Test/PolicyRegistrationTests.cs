using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi_Test;

public class PolicyRegistrationTests(PortfolioApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public void EditorOrAdminPolicy_IsRegistered()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();

        // Act
        var policy = provider.GetPolicyAsync("EditorOrAdmin").GetAwaiter().GetResult();

        // Assert
        Assert.NotNull(policy);
        Assert.NotEmpty(policy!.Requirements);
    }

    [Fact]
    public void RolesGastEditorAdmin_AreRegistered()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

        // Act & Assert
        foreach (var role in new[] { "Gast", "Editor", "Admin" })
            Assert.True(roleManager.RoleExistsAsync(role).GetAwaiter().GetResult(),
                $"Rol '{role}' ontbreekt.");
    }
}
