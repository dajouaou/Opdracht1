using System.Net;
using System.Net.Http.Json;
using PortfolioApi.DTOs;

namespace PortfolioApi_Test;

public class ProjectsCrudTests(PortfolioApiFactory factory) : IntegrationTestBase(factory)
{
    // ---------- READ ----------

    [Fact]
    public async Task GetAll_Anonymous_ReturnsOkWithList()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<ProjectReadDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOkWithMatchingId()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/projects", TestData.Project());
        var created = await create.Content.ReadFromJsonAsync<ProjectReadDto>();

        // Act
        var response = await client.GetAsync($"/api/projects/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Testproject", fetched.Titel);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- CREATE ----------

    [Fact]
    public async Task Create_AsEditor_ReturnsCreatedWithLocationAndBody()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", TestData.Project("Nieuw project"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var created = await response.Content.ReadFromJsonAsync<ProjectReadDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Nieuw project", created.Titel);
        Assert.Equal("Web", created.Categorie);
    }

    [Fact]
    public async Task Create_AsAdmin_ReturnsCreated()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("admin"), ValidPassword, "Admin");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", TestData.Project());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // ---------- UPDATE ----------

    [Fact]
    public async Task Update_Existing_ReturnsNoContentAndPersists()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/projects", TestData.Project("Oud"));
        var created = await create.Content.ReadFromJsonAsync<ProjectReadDto>();

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{created!.Id}", new
        {
            titel = "Aangepast",
            beschrijving = "Nieuwe beschrijving",
            categorie = "API",
            gitHubUrl = (string?)null,
            datum = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetFromJsonAsync<ProjectReadDto>($"/api/projects/{created.Id}");
        Assert.Equal("Aangepast", fetched!.Titel);
        Assert.Equal("API", fetched.Categorie);
        Assert.Null(fetched.GitHubUrl);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/projects/999999", new
        {
            titel = "x",
            beschrijving = "b",
            categorie = "c",
            gitHubUrl = (string?)null,
            datum = DateTime.UtcNow
        });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_Existing_ReturnsNoContentAndRemoves()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/projects", TestData.Project());
        var created = await create.Content.ReadFromJsonAsync<ProjectReadDto>();

        // Act
        var response = await client.DeleteAsync($"/api/projects/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var after = await client.GetAsync($"/api/projects/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, after.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.DeleteAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
