using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi_Test;

public class ProjectServiceTests
{
    private static PortfolioDbContext NewDb() =>
        new(new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateAsync_MapsAllFieldsAndAssignsId()
    {
        // Arrange
        var db = NewDb();
        var service = new ProjectService(db);
        var dto = new ProjectCreateDto("Titel", "Beschrijving", "Categorie",
            "https://github.com/x/y", new DateTime(2026, 1, 1));

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("Titel", result.Titel);
        Assert.Equal("Beschrijving", result.Beschrijving);
        Assert.Equal("Categorie", result.Categorie);
        Assert.Equal("https://github.com/x/y", result.GitHubUrl);
        Assert.Equal(1, await db.Projects.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjectsOrderedById()
    {
        // Arrange
        var db = NewDb();
        var service = new ProjectService(db);
        await service.CreateAsync(new ProjectCreateDto("Eerste", "b", "c", null, DateTime.UtcNow));
        await service.CreateAsync(new ProjectCreateDto("Tweede", "b", "c", null, DateTime.UtcNow));

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Eerste", result[0].Titel);
        Assert.True(result[0].Id < result[1].Id);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsDto()
    {
        // Arrange
        var db = NewDb();
        var service = new ProjectService(db);
        var created = await service.CreateAsync(
            new ProjectCreateDto("T", "b", "c", null, DateTime.UtcNow));

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        // Arrange
        var service = new ProjectService(NewDb());

        // Act
        var result = await service.GetByIdAsync(42);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsTrueAndPersists()
    {
        // Arrange
        var db = NewDb();
        var service = new ProjectService(db);
        var created = await service.CreateAsync(
            new ProjectCreateDto("Oud", "b", "c", null, DateTime.UtcNow));

        // Act
        var result = await service.UpdateAsync(created.Id,
            new ProjectUpdateDto("Nieuw", "b2", "c2", "https://gh", DateTime.UtcNow));

        // Assert
        Assert.True(result);
        var fetched = await service.GetByIdAsync(created.Id);
        Assert.Equal("Nieuw", fetched!.Titel);
        Assert.Equal("c2", fetched.Categorie);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ReturnsFalse()
    {
        // Arrange
        var service = new ProjectService(NewDb());

        // Act
        var result = await service.UpdateAsync(42,
            new ProjectUpdateDto("T", "b", "c", null, DateTime.UtcNow));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrueAndRemoves()
    {
        // Arrange
        var db = NewDb();
        var service = new ProjectService(db);
        var created = await service.CreateAsync(
            new ProjectCreateDto("T", "b", "c", null, DateTime.UtcNow));

        // Act
        var result = await service.DeleteAsync(created.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await db.Projects.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ReturnsFalse()
    {
        // Arrange
        var service = new ProjectService(NewDb());

        // Act
        var result = await service.DeleteAsync(42);

        // Assert
        Assert.False(result);
    }
}
