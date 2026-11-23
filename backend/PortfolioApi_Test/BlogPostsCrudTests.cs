using System.Net;
using System.Net.Http.Json;
using PortfolioApi.DTOs;

namespace PortfolioApi_Test;

public class BlogPostsCrudTests(PortfolioApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAll_Anonymous_ReturnsOkWithList()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<BlogPostReadDto>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetAll_ReturnsPostsOrderedByPublicatiedatumDescending()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        await client.PostAsJsonAsync("/api/blogposts",
            TestData.BlogPost("Oudste", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
        await client.PostAsJsonAsync("/api/blogposts",
            TestData.BlogPost("Nieuwste", new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)));

        // Act
        var list = await client.GetFromJsonAsync<List<BlogPostReadDto>>("/api/blogposts");

        // Assert
        Assert.NotNull(list);
        var titels = list!.Select(p => p.Titel).ToList();
        var indexOud = titels.IndexOf("Oudste");
        var indexNieuw = titels.IndexOf("Nieuwste");
        Assert.True(indexNieuw >= 0 && indexOud >= 0);
        Assert.True(indexNieuw < indexOud,
            "Nieuwste post moet vóór de oudste staan (aflopend op Publicatiedatum).");
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost());
        var created = await create.Content.ReadFromJsonAsync<BlogPostReadDto>();

        // Act
        var response = await client.GetAsync($"/api/blogposts/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<BlogPostReadDto>();
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_AsEditor_ReturnsCreatedWithLocation()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost("Nieuwe post"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var created = await response.Content.ReadFromJsonAsync<BlogPostReadDto>();
        Assert.Equal("Nieuwe post", created!.Titel);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task Create_AsAdmin_ReturnsCreated()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("admin"), ValidPassword, "Admin");

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContentAndPersists()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost("Oud"));
        var created = await create.Content.ReadFromJsonAsync<BlogPostReadDto>();

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{created!.Id}", new
        {
            titel = "Aangepast",
            inhoud = "Nieuwe inhoud",
            publicatiedatum = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetFromJsonAsync<BlogPostReadDto>($"/api/blogposts/{created.Id}");
        Assert.Equal("Aangepast", fetched!.Titel);
        Assert.Equal("Nieuwe inhoud", fetched.Inhoud);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/999999", new
        {
            titel = "x",
            inhoud = "i",
            publicatiedatum = DateTime.UtcNow
        });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContentAndRemoves()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");
        var create = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost());
        var created = await create.Content.ReadFromJsonAsync<BlogPostReadDto>();

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var after = await client.GetAsync($"/api/blogposts/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, after.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("editor"), ValidPassword, "Editor");

        // Act
        var response = await client.DeleteAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
