using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Context;

public class PortfolioDbContext : IdentityDbContext<IdentityUser>
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<About> Abouts => Set<About>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Domain.Entities.SEO> SEOs => Set<Domain.Entities.SEO>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(p => p.Title).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Description).IsRequired();
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Category).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Slug).IsRequired().HasMaxLength(250);
            entity.HasIndex(b => b.Slug).IsUnique();
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.Property(r => r.FileName).IsRequired().HasMaxLength(250);
            entity.Property(r => r.FilePath).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.Property(s => s.Key).IsRequired().HasMaxLength(100);
            entity.HasIndex(s => s.Key).IsUnique();
        });

        modelBuilder.Entity<Domain.Entities.SEO>(entity =>
        {
            entity.Property(s => s.PageName).IsRequired().HasMaxLength(100);
            entity.HasIndex(s => s.PageName).IsUnique();
        });
    }
}
