using System.Net;
using System.Net.Http.Json;
using PortfolioApi.DTOs;

namespace PortfolioApi.Tests;

// elke test krijgt eigen lege database.
public class BlogPostsTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();

    public void Dispose()
    {
        _factory.Dispose();
    }

    // geldige blogpost om aan te maken
    private static BlogPostCreateDto GeldigeBlogPost(string titel = "Mijn blogpost", DateTime? datum = null)
    {
        return new BlogPostCreateDto
        {
            Titel = titel,
            Inhoud = "De inhoud van de blogpost",
            Publicatiedatum = datum ?? new DateTime(2025, 1, 1)
        };
    }

    // zelfde gegevens voor een wijziging (put gebruikt een ander dto)
    private static BlogPostUpdateDto GeldigeWijziging(string titel)
    {
        return new BlogPostUpdateDto
        {
            Titel = titel,
            Inhoud = "De inhoud van de blogpost",
            Publicatiedatum = new DateTime(2025, 1, 1)
        };
    }

    // maakt als Editor een blogpost aan en geeft het id terug
    private async Task<int> MaakBlogPostAanAsync(HttpClient editor, string titel = "Mijn blogpost", DateTime? datum = null)
    {
        var response = await editor.PostAsJsonAsync("/api/blogposts", GeldigeBlogPost(titel, datum));
        var blogPost = await response.Content.ReadFromJsonAsync<BlogPostReadDto>();
        return blogPost!.Id;
    }

    // get

    [Fact]
    public async Task GetBlogPosts_Anoniem_Geeft200EnLijst()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        await MaakBlogPostAanAsync(editor, "Eerste");
        await MaakBlogPostAanAsync(editor, "Tweede");
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var lijst = await response.Content.ReadFromJsonAsync<List<BlogPostReadDto>>();
        Assert.Equal(2, lijst!.Count);
    }

    [Fact]
    public async Task GetBlogPosts_SorteertOpPublicatiedatumAflopend()
    {
        // Arrange: bewust niet op volgorde aangemaakt
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        await MaakBlogPostAanAsync(editor, "Oud", new DateTime(2023, 1, 1));
        await MaakBlogPostAanAsync(editor, "Nieuw", new DateTime(2025, 1, 1));
        await MaakBlogPostAanAsync(editor, "Midden", new DateTime(2024, 1, 1));

        // Act
        var lijst = await editor.GetFromJsonAsync<List<BlogPostReadDto>>("/api/blogposts");

        // Assert: nieuwste eerst
        Assert.Equal("Nieuw", lijst![0].Titel);
        Assert.Equal("Midden", lijst[1].Titel);
        Assert.Equal("Oud", lijst[2].Titel);
    }

    [Fact]
    public async Task GetBlogPostById_Bestaand_Geeft200EnJuisteData()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor, "Mijn titel");
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.GetAsync($"/api/blogposts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogPost = await response.Content.ReadFromJsonAsync<BlogPostReadDto>();
        Assert.Equal("Mijn titel", blogPost!.Titel);
        Assert.Equal("De inhoud van de blogpost", blogPost.Inhoud);
    }

    [Fact]
    public async Task GetBlogPostById_Onbekend_Geeft404()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // post

    [Fact]
    public async Task PostBlogPost_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", GeldigeBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogPost_AlsGast_Geeft403()
    {
        // Arrange
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.PostAsJsonAsync("/api/blogposts", GeldigeBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogPost_AlsEditor_Geeft201EnWordtOpgeslagen()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.PostAsJsonAsync("/api/blogposts", GeldigeBlogPost("Nieuwe post"));

        // Assert: statuscode en inhoud
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<BlogPostReadDto>();
        Assert.Equal("Nieuwe post", aangemaakt!.Titel);

        // Assert: de blogpost staat echt in de database
        var opgehaald = await editor.GetAsync($"/api/blogposts/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.OK, opgehaald.StatusCode);
    }

    [Fact]
    public async Task PostBlogPost_AlsAdmin_Geeft201()
    {
        // Arrange
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.PostAsJsonAsync("/api/blogposts", GeldigeBlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogPost_OngeldigeInvoer_Geeft400EnSlaatNietsOp()
    {
        // Arrange: lege titel is niet toegestaan ([Required])
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var ongeldig = GeldigeBlogPost("");

        // Act
        var response = await editor.PostAsJsonAsync("/api/blogposts", ongeldig);

        // Assert: foutmelding, en de lijst is nog leeg
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var lijst = await editor.GetFromJsonAsync<List<BlogPostReadDto>>("/api/blogposts");
        Assert.Empty(lijst!);
    }
    [Fact]
    public async Task PostBlogPost_TeLangeInhoud_Geeft400EnSlaatNietsOp()
    {
    // Arrange: de inhoud mag maximaal 10000 tekens zijn, dit zijn er 10001
    var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
    var teLang = GeldigeBlogPost();
    teLang.Inhoud = new string('a', 10001);

    // Act
    var response = await editor.PostAsJsonAsync("/api/blogposts", teLang);

    // Assert: foutmelding, en de lijst is nog leeg
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    var lijst = await editor.GetFromJsonAsync<List<BlogPostReadDto>>("/api/blogposts");
    Assert.Empty(lijst!);}

    // put

    [Fact]
    public async Task PutBlogPost_AlsEditor_Geeft204EnWijzigtGegevens()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor, "Oude titel");

        // Act
        var response = await editor.PutAsJsonAsync($"/api/blogposts/{id}", GeldigeWijziging("Nieuwe titel"));

        // Assert: statuscode, en daarna toont GET de nieuwe titel
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var blogPost = await editor.GetFromJsonAsync<BlogPostReadDto>($"/api/blogposts/{id}");
        Assert.Equal("Nieuwe titel", blogPost!.Titel);
    }

    [Fact]
    public async Task PutBlogPost_AlsAdmin_Geeft204()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.PutAsJsonAsync($"/api/blogposts/{id}", GeldigeWijziging("Door admin"));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogPost_Onbekend_Geeft404()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.PutAsJsonAsync("/api/blogposts/9999", GeldigeWijziging("Titel"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogPost_ZonderLogin_Geeft401()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.PutAsJsonAsync($"/api/blogposts/{id}", GeldigeWijziging("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogPost_AlsGast_Geeft403()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.PutAsJsonAsync($"/api/blogposts/{id}", GeldigeWijziging("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogPost_OngeldigeInvoer_Geeft400()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);

        // Act: lege titel
        var response = await editor.PutAsJsonAsync($"/api/blogposts/{id}", GeldigeWijziging(""));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // delete

    [Fact]
    public async Task DeleteBlogPost_AlsEditor_Geeft204EnVerwijdertHet()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);

        // Act
        var response = await editor.DeleteAsync($"/api/blogposts/{id}");

        // Assert: statuscode, en daarna geeft GET 404
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var daarna = await editor.GetAsync($"/api/blogposts/{id}");
        Assert.Equal(HttpStatusCode.NotFound, daarna.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogPost_AlsAdmin_Geeft204()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.DeleteAsync($"/api/blogposts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogPost_Onbekend_Geeft404()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.DeleteAsync("/api/blogposts/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogPost_ZonderLogin_Geeft401()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.DeleteAsync($"/api/blogposts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogPost_AlsGast_Geeft403()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakBlogPostAanAsync(editor);
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.DeleteAsync($"/api/blogposts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
