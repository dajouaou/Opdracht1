using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.DTOs;

public class BlogPostUpdateDto
{
    [Required, StringLength(160)]
    public string Titel { get; set; } = string.Empty;

    [Required, StringLength(10000)]
    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}