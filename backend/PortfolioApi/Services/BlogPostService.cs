using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class BlogPostService(PortfolioDbContext db) : IBlogPostService
{
    public async Task<List<BlogPostReadDto>> GetAllAsync() =>
        await db.BlogPosts.AsNoTracking()
            .OrderByDescending(b => b.Publicatiedatum)
            .Select(b => ToDto(b))
            .ToListAsync();

    public async Task<BlogPostReadDto?> GetByIdAsync(int id)
    {
        var post = await db.BlogPosts.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return post is null ? null : ToDto(post);
    }

    public async Task<BlogPostReadDto> CreateAsync(BlogPostCreateDto dto)
    {
        var post = new BlogPost
        {
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };

        post.Publicatiedatum = DateTime.Now;
        db.BlogPosts.Add(post);
        await db.SaveChangesAsync();

        return ToDto(post);
    }

    public async Task<bool> UpdateAsync(int id, BlogPostUpdateDto dto)
    {
        var post = await db.BlogPosts.FindAsync(id);
        if (post is null) return false;

        post.Titel = dto.Titel;
        post.Inhoud = dto.Inhoud;
        post.Publicatiedatum = dto.Publicatiedatum;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await db.BlogPosts.FindAsync(id);
        if (post is null) return false;

        db.BlogPosts.Remove(post);
        await db.SaveChangesAsync();
        return true;
    }

    // hier zet ik de databaseblogpost om naar gegevens voor de api
    private static BlogPostReadDto ToDto(BlogPost b) => new()
    {
        Id = b.Id,
        Titel = b.Titel,
        Inhoud = b.Inhoud,
        Publicatiedatum = b.Publicatiedatum
    };
}