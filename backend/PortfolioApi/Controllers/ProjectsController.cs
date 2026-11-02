using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectReadDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectReadDto>> GetById(int id)
    {
        var project = await service.GetByIdAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> Create(ProjectCreateDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto dto) =>
        await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await service.DeleteAsync(id) ? NoContent() : NotFound();
}