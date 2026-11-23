using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi_Test;

public class BlogPostServiceTests
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
        var service = new BlogPostService(db);
        var dto = new BlogPostCreateDto("Titel", "Inhoud", new DateTime(2026, 1, 1));

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("Titel", result.Titel);
        Assert.Equal("Inhoud", result.Inhoud);
        Assert.Equal(new DateTime(2026, 1, 1), result.Publicatiedatum);
        Assert.Equal(1, await db.BlogPosts.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_OrdersByPublicatiedatumDescending()
    {
        // Arrange
        var db = NewDb();
        var service = new BlogPostService(db);
        await service.CreateAsync(new BlogPostCreateDto("Oud", "i", new DateTime(2024, 1, 1)));
        await service.CreateAsync(new BlogPostCreateDto("Nieuw", "i", new DateTime(2026, 1, 1)));

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Nieuw", result[0].Titel);
        Assert.Equal("Oud", result[1].Titel);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsDto()
    {
        // Arrange
        var db = NewDb();
        var service = new BlogPostService(db);
        var created = await service.CreateAsync(
            new BlogPostCreateDto("T", "i", DateTime.UtcNow));

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal("T", result.Titel);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        // Arrange
        var service = new BlogPostService(NewDb());

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
        var service = new BlogPostService(db);
        var created = await service.CreateAsync(
            new BlogPostCreateDto("Oud", "oude inhoud", new DateTime(2025, 1, 1)));

        // Act
        var result = await service.UpdateAsync(created.Id,
            new BlogPostUpdateDto("Nieuw", "nieuwe inhoud", new DateTime(2026, 5, 5)));

        // Assert
        Assert.True(result);
        var fetched = await service.GetByIdAsync(created.Id);
        Assert.Equal("Nieuw", fetched!.Titel);
        Assert.Equal("nieuwe inhoud", fetched.Inhoud);
        Assert.Equal(new DateTime(2026, 5, 5), fetched.Publicatiedatum);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ReturnsFalse()
    {
        // Arrange
        var service = new BlogPostService(NewDb());

        // Act
        var result = await service.UpdateAsync(42,
            new BlogPostUpdateDto("T", "i", DateTime.UtcNow));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrueAndRemoves()
    {
        // Arrange
        var db = NewDb();
        var service = new BlogPostService(db);
        var created = await service.CreateAsync(
            new BlogPostCreateDto("T", "i", DateTime.UtcNow));

        // Act
        var result = await service.DeleteAsync(created.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await db.BlogPosts.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ReturnsFalse()
    {
        // Arrange
        var service = new BlogPostService(NewDb());

        // Act
        var result = await service.DeleteAsync(42);

        // Assert
        Assert.False(result);
    }
}
