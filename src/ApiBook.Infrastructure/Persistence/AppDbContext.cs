using ApiBook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiBook.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Command> Commands => Set<Command>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(entity =>
        {
            entity.ToTable("platforms");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Publisher)
                .HasMaxLength(100)
                .IsRequired();

            // 🔥 IMPORTANT: Use field access for _commands
            entity.Metadata
                .FindNavigation(nameof(Platform.Commands))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 🔥 Soft delete filter
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.ToTable("commands");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.HowTo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.CommandLine)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasOne(x => x.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(x => x.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }

    // 🔥 BONUS: Audit handling
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is Platform platform)
            {
                if (entry.State == EntityState.Added)
                    platform.GetType().GetProperty("CreatedAt")?.SetValue(platform, DateTime.UtcNow);

                if (entry.State == EntityState.Modified)
                    platform.GetType().GetProperty("UpdatedAt")?.SetValue(platform, DateTime.UtcNow);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}