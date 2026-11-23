using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PortfolioApi.Data;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
    : IdentityDbContext<IdentityUser>(options) // zorgt voor Identity tabellen (Identityuser is standaard gebruikersmodel van asp.net core identity)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // zorgt dat identity eigen tabellen en relaties configureert

        modelBuilder.Entity<Project>()
            .Property(p => p.Titel)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<Project>()
            .Property(p => p.Beschrijving)
            .HasMaxLength(2000)
            .IsRequired();

        modelBuilder.Entity<Project>()
            .Property(p => p.Categorie)
            .HasMaxLength(80)
            .IsRequired();

        modelBuilder.Entity<Project>()
            .Property(p => p.GitHubUrl)
            .HasMaxLength(500);

        modelBuilder.Entity<BlogPost>()
            .Property(b => b.Titel)
            .HasMaxLength(160)
            .IsRequired();

        modelBuilder.Entity<BlogPost>()
            .Property(b => b.Inhoud)
            .HasMaxLength(10000)
            .IsRequired();
    }
}