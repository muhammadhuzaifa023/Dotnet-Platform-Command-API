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

    // 🔥 IMPORTANT: Tracking ON (no AsNoTracking)
    public async Task<Platform?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Platforms
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    // ✅ ADD (no SaveChanges)
    public async Task<Platform> AddAsync(Platform platform, CancellationToken cancellationToken = default)
    {
        await dbContext.Platforms.AddAsync(platform, cancellationToken);
        return platform;
    }

    // ✅ UPDATE (Domain already updated in Service)
    public Task<bool> UpdateAsync(Platform platform, CancellationToken cancellationToken = default)
    {
        // EF Core already tracking entity
        return Task.FromResult(true);
    }

    // ✅ DELETE (no SaveChanges)
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Platforms
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (existing is null)
            return false;

        dbContext.Platforms.Remove(existing);
        return true;
    }
}