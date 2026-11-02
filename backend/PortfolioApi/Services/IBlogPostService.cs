using PortfolioApi.DTOs;

namespace PortfolioApi.Services;

public interface IBlogPostService
{
    Task<List<BlogPostReadDto>> GetAllAsync();
    Task<BlogPostReadDto?> GetByIdAsync(int id);
    Task<BlogPostReadDto> CreateAsync(BlogPostCreateDto dto);
    Task<bool> UpdateAsync(int id, BlogPostUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}