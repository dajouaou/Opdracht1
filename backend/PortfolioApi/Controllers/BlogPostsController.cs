using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogPostsController(IBlogPostService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BlogPostReadDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogPostReadDto>> GetById(int id)
    {
        var post = await service.GetByIdAsync(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<BlogPostReadDto>> Create(BlogPostCreateDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BlogPostUpdateDto dto) =>
        await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await service.DeleteAsync(id) ? NoContent() : NotFound();
}