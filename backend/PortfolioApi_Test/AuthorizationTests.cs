using System.Net;
using System.Net.Http.Json;

namespace PortfolioApi_Test;

public class AuthorizationTests(PortfolioApiFactory factory) : IntegrationTestBase(factory)
{
    public static IEnumerable<object[]> ProtectedWriteRoutes()
    {
        yield return new object[] { "POST", "/api/projects" };
        yield return new object[] { "PUT", "/api/projects/1" };
        yield return new object[] { "DELETE", "/api/projects/1" };
        yield return new object[] { "POST", "/api/blogposts" };
        yield return new object[] { "PUT", "/api/blogposts/1" };
        yield return new object[] { "DELETE", "/api/blogposts/1" };
    }

    public static IEnumerable<object[]> PublicReadRoutes()
    {
        yield return new object[] { "/api/projects" };
        yield return new object[] { "/api/projects/1" };
        yield return new object[] { "/api/blogposts" };
        yield return new object[] { "/api/blogposts/1" };
    }

    [Theory]
    [MemberData(nameof(ProtectedWriteRoutes))]
    public async Task Write_WithoutLogin_Returns401(string method, string url)
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await SendAsync(client, method, url);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(ProtectedWriteRoutes))]
    public async Task Write_AsGast_Returns403(string method, string url)
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail("gast"), ValidPassword, "Gast");

        // Act
        var response = await SendAsync(client, method, url);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(PublicReadRoutes))]
    public async Task Read_WithoutLogin_IsNeverUnauthorized(string url)
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound,
            $"Verwacht 200 of 404, kreeg {(int)response.StatusCode}.");
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task WriteProject_AsEditorOrAdmin_IsAllowed(string role)
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail(role.ToLowerInvariant()), ValidPassword, role);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", TestData.Project());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task WriteBlogPost_AsEditorOrAdmin_IsAllowed(string role)
    {
        // Arrange
        var client = await CreateLoggedInClientAsync(
            UniqueEmail(role.ToLowerInvariant()), ValidPassword, role);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", TestData.BlogPost());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static Task<HttpResponseMessage> SendAsync(HttpClient client, string method, string url)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method is "POST" or "PUT")
        {
            request.Content = JsonContent.Create(new
            {
                titel = "x",
                inhoud = "i",
                beschrijving = "b",
                categorie = "c",
                gitHubUrl = (string?)null,
                datum = DateTime.UtcNow,
                publicatiedatum = DateTime.UtcNow
            });
        }
        return client.SendAsync(request);
    }
}
