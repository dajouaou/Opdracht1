using System.Net;
using System.Net.Http.Json;
using PortfolioApi.DTOs;

namespace PortfolioApi.Tests;

// elke test maakt zijn eigen factory, dus elke test begint met een lege database.
public class ProjectsTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();

    public void Dispose()
    {
        _factory.Dispose();
    }

    // een geldig project om aan te maken
    private static ProjectCreateDto GeldigProject(string titel = "Mijn project")
    {
        return new ProjectCreateDto
        {
            Titel = titel,
            Beschrijving = "Een beschrijving",
            Categorie = "Web",
            GitHubUrl = "https://github.com/test/project",
            Datum = new DateTime(2025, 1, 1)
        };
    }

    [Fact]
public async Task PostProject_TeLangeBeschrijving_Geeft400EnSlaatNietsOp()
{
    // Arrange: de beschrijving mag maximaal 2000 tekens zijn, dit zijn er 2001
    var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
    var teLang = GeldigProject();
    teLang.Beschrijving = new string('a', 2001);

    // Act
    var response = await editor.PostAsJsonAsync("/api/projects", teLang);

    // Assert: foutmelding, en de lijst is nog leeg
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    var lijst = await editor.GetFromJsonAsync<List<ProjectReadDto>>("/api/projects");
    Assert.Empty(lijst!);
}

[Fact]
public async Task PostProject_TeLangeCategorie_Geeft400EnSlaatNietsOp()
{
    // Arrange: de categorie mag maximaal 80 tekens zijn, dit zijn er 81
    var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
    var teLang = GeldigProject();
    teLang.Categorie = new string('a', 81);

    // Act
    var response = await editor.PostAsJsonAsync("/api/projects", teLang);

    // Assert: foutmelding, en de lijst is nog leeg
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    var lijst = await editor.GetFromJsonAsync<List<ProjectReadDto>>("/api/projects");
    Assert.Empty(lijst!);
}

[Fact]
public async Task PostProject_TeLangeGitHubUrl_Geeft400EnSlaatNietsOp()
{
    // Arrange: de GitHub-url mag maximaal 500 tekens zijn, dit zijn er meer dan 500
    var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
    var teLang = GeldigProject();
    teLang.GitHubUrl = "https://github.com/" + new string('a', 500);

    // Act
    var response = await editor.PostAsJsonAsync("/api/projects", teLang);

    // Assert: foutmelding, en de lijst is nog leeg
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    var lijst = await editor.GetFromJsonAsync<List<ProjectReadDto>>("/api/projects");
    Assert.Empty(lijst!);
}

    // dezelfde gegevens voor een wijziging (PUT gebruikt een ander DTO)
    private static ProjectUpdateDto GeldigeWijziging(string titel)
    {
        return new ProjectUpdateDto
        {
            Titel = titel,
            Beschrijving = "Een beschrijving",
            Categorie = "Web",
            GitHubUrl = "https://github.com/test/project",
            Datum = new DateTime(2025, 1, 1)
        };
    }

    // maakt als Editor een project aan en geeft het id terug
    private async Task<int> MaakProjectAanAsync(HttpClient editor, string titel = "Mijn project")
    {
        var response = await editor.PostAsJsonAsync("/api/projects", GeldigProject(titel));
        var project = await response.Content.ReadFromJsonAsync<ProjectReadDto>();
        return project!.Id;
    }

    // get

    [Fact]
    public async Task GetProjects_Anoniem_Geeft200EnLijst()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        await MaakProjectAanAsync(editor, "Eerste");
        await MaakProjectAanAsync(editor, "Tweede");
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var lijst = await response.Content.ReadFromJsonAsync<List<ProjectReadDto>>();
        Assert.Equal(2, lijst!.Count);
        Assert.Equal("Eerste", lijst[0].Titel);
    }

    [Fact]
    public async Task GetProjectById_Bestaand_Geeft200EnJuisteData()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor, "Mijn titel");
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.GetAsync($"/api/projects/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var project = await response.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.Equal("Mijn titel", project!.Titel);
        Assert.Equal("Web", project.Categorie);
        Assert.Equal("https://github.com/test/project", project.GitHubUrl);
    }

    [Fact]
    public async Task GetProjectById_Onbekend_Geeft404()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/projects/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // post

    [Fact]
    public async Task PostProject_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_AlsGast_Geeft403()
    {
        // Arrange
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_AlsEditor_Geeft201EnWordtOpgeslagen()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.PostAsJsonAsync("/api/projects", GeldigProject("Nieuw project"));

        // Assert: statuscode en inhoud
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.Equal("Nieuw project", aangemaakt!.Titel);

        // Assert: het project staat echt in de database
        var opgehaald = await editor.GetAsync($"/api/projects/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.OK, opgehaald.StatusCode);
    }

    [Fact]
    public async Task PostProject_AlsAdmin_Geeft201()
    {
        // Arrange
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_OngeldigeInvoer_Geeft400EnSlaatNietsOp()
    {
        // Arrange: lege titel is niet toegestaan ([Required])
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var ongeldig = GeldigProject("");

        // Act
        var response = await editor.PostAsJsonAsync("/api/projects", ongeldig);

        // Assert: foutmelding, en de lijst is nog leeg
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var lijst = await editor.GetFromJsonAsync<List<ProjectReadDto>>("/api/projects");
        Assert.Empty(lijst!);
    }

    // put

    [Fact]
    public async Task PutProject_AlsEditor_Geeft204EnWijzigtGegevens()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor, "Oude titel");

        // Act
        var response = await editor.PutAsJsonAsync($"/api/projects/{id}", GeldigeWijziging("Nieuwe titel"));

        // Assert: statuscode, en daarna toont GET de nieuwe titel
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var project = await editor.GetFromJsonAsync<ProjectReadDto>($"/api/projects/{id}");
        Assert.Equal("Nieuwe titel", project!.Titel);
    }

    [Fact]
    public async Task PutProject_AlsAdmin_Geeft204()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.PutAsJsonAsync($"/api/projects/{id}", GeldigeWijziging("Door admin"));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_Onbekend_Geeft404()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.PutAsJsonAsync("/api/projects/9999", GeldigeWijziging("Titel"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_ZonderLogin_Geeft401()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.PutAsJsonAsync($"/api/projects/{id}", GeldigeWijziging("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_AlsGast_Geeft403()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.PutAsJsonAsync($"/api/projects/{id}", GeldigeWijziging("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_OngeldigeInvoer_Geeft400()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);

        // Act: lege titel
        var response = await editor.PutAsJsonAsync($"/api/projects/{id}", GeldigeWijziging(""));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // delete

    [Fact]
    public async Task DeleteProject_AlsEditor_Geeft204EnVerwijdertHet()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);

        // Act
        var response = await editor.DeleteAsync($"/api/projects/{id}");

        // Assert: statuscode, en daarna geeft GET 404
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var daarna = await editor.GetAsync($"/api/projects/{id}");
        Assert.Equal(HttpStatusCode.NotFound, daarna.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_AlsAdmin_Geeft204()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var admin = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Admin");

        // Act
        var response = await admin.DeleteAsync($"/api/projects/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_Onbekend_Geeft404()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");

        // Act
        var response = await editor.DeleteAsync("/api/projects/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_ZonderLogin_Geeft401()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var anoniem = _factory.MaakClient();

        // Act
        var response = await anoniem.DeleteAsync($"/api/projects/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_AlsGast_Geeft403()
    {
        // Arrange
        var editor = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Editor");
        var id = await MaakProjectAanAsync(editor);
        var gast = await TestHelpers.MaakIngelogdeClientAsync(_factory, "Gast");

        // Act
        var response = await gast.DeleteAsync($"/api/projects/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
