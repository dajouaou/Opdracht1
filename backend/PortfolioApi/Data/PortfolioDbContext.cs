using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Data;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().Property(p => p.Titel).HasMaxLength(120).IsRequired();
        modelBuilder.Entity<Project>().Property(p => p.Beschrijving).HasMaxLength(2000).IsRequired();
        modelBuilder.Entity<Project>().Property(p => p.Categorie).HasMaxLength(80).IsRequired();
        modelBuilder.Entity<Project>().Property(p => p.GitHubUrl).HasMaxLength(500);

        modelBuilder.Entity<BlogPost>().Property(b => b.Titel).HasMaxLength(160).IsRequired();
        modelBuilder.Entity<BlogPost>().Property(b => b.Inhoud).HasMaxLength(10000).IsRequired();
    }
}