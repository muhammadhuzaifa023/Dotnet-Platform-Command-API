using ApiBook.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Publisher).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.ToTable("commands");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.HowTo).HasMaxLength(200).IsRequired();
            entity.Property(x => x.CommandLine).HasMaxLength(200).IsRequired();
            entity.HasOne(x => x.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(x => x.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
