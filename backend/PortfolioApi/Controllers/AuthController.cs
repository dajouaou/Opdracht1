
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<IdentityUser> userManager, // maakt gebruikers aan en beheert de rollen die ze hebben
    SignInManager<IdentityUser> signInManager) : ControllerBase // controleert wachtwoord & verzorgt inloggen
{
    [HttpPost("register")]
    [AllowAnonymous] // registratie en login zonder acc toegankelijk
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(
            user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await userManager.AddToRoleAsync(user, "Gast");

        return Ok(new
        {
            message = "Account succesvol aangemaakt."
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                message = "Ongeldig e-mailadres of wachtwoord."
            });
        }

        return Ok(new
        {
            message = "Succesvol ingelogd."
        });
    }

    [HttpPost("logout")]
    [Authorize] //alleen ingelogde gebruikers de logout route kunnen gebruiken
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return Ok(new
        {
            message = "Succesvol uitgelogd."
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            email = User.Identity?.Name,
            roles = User.Claims
                .Where(c => c.Type ==
                    System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
        });
    }
}
