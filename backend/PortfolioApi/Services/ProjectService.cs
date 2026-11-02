using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class ProjectService(PortfolioDbContext db) : IProjectService
{
    public async Task<List<ProjectReadDto>> GetAllAsync() =>
        await db.Projects.AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => ToDto(p))
            .ToListAsync();

    public async Task<ProjectReadDto?> GetByIdAsync(int id)
    {
        var project = await db.Projects.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return project is null ? null : ToDto(project);
    }

   public async Task<ProjectReadDto> CreateAsync(ProjectCreateDto dto)
{
    var project = new Project
    {
        Titel = dto.Titel,
        Beschrijving = dto.Beschrijving,
        Categorie = dto.Categorie,
        GitHubUrl = dto.GitHubUrl,
        Datum = dto.Datum
    };

    db.Projects.Add(project);
    await db.SaveChangesAsync();

    return ToDto(project);
}

    public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return false;

        project.Titel = dto.Titel;
        project.Beschrijving = dto.Beschrijving;
        project.Categorie = dto.Categorie;
        project.GitHubUrl = dto.GitHubUrl;
        project.Datum = dto.Datum;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    // hier zet ik het databaseproject om naar gegevens voor de api
    private static ProjectReadDto ToDto(Project p) => new()
    {
        Id = p.Id,
        Titel = p.Titel,
        Beschrijving = p.Beschrijving,
        Categorie = p.Categorie,
        GitHubUrl = p.GitHubUrl,
        Datum = p.Datum
    };
}