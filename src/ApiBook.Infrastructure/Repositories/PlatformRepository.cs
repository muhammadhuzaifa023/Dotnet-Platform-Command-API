using ApiBook.Application.Contracts;
using ApiBook.Domain.Entities;
using ApiBook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiBook.Infrastructure.Repositories;

public class PlatformRepository(AppDbContext dbContext) : IPlatformRepository
{
    public async Task<IReadOnlyList<Platform>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Platforms
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Platform?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Platforms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    // ✅ Nayi methods
    public async Task<Platform> AddAsync(Platform platform, CancellationToken cancellationToken = default)
    {
        dbContext.Platforms.Add(platform);
        await dbContext.SaveChangesAsync(cancellationToken);
        return platform;
    }

    public async Task<bool> UpdateAsync(Platform platform, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Platforms
            .FirstOrDefaultAsync(x => x.Id == platform.Id, cancellationToken);
        if (existing is null) return false;

        existing.Name = platform.Name;
        existing.Publisher = platform.Publisher;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Platforms
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;

        dbContext.Platforms.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
