using Microsoft.AspNetCore.Identity;

public static class RolSeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Gast", "Editor", "Admin" }; // de rollen in question

        foreach (var role in roles) // controle of rollen al bestaan
        {
            if (!await roleManager.RoleExistsAsync(role))  // als rol niet bestaat
            {
                await roleManager.CreateAsync( // wachten tot de task uitgevoerd is
                    new IdentityRole(role)); // maak rol aan
            }
        }
    }
}
