using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.DTOs;

public class ProjectUpdateDto
{
    [Required, StringLength(120)]
    public string Titel { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Beschrijving { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Categorie { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? GitHubUrl { get; set; }

    public DateTime Datum { get; set; }
}