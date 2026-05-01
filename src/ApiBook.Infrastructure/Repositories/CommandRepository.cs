using ApiBook.Application.Contracts;
using ApiBook.Domain.Entities;
using ApiBook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiBook.Infrastructure.Repositories;

public class CommandRepository(AppDbContext dbContext) : ICommandRepository
{
    public async Task<IReadOnlyList<Command>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Commands
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Command>> GetByPlatformIdAsync(int platformId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Commands
            .AsNoTracking()
            .Where(x => x.PlatformId == platformId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    // 🔥 IMPORTANT: Tracking ON (no AsNoTracking)
    public async Task<Command?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Commands
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    // ✅ ADD only (no SaveChanges)
    public async Task<Command> AddAsync(Command command, CancellationToken cancellationToken = default)
    {
        await dbContext.Commands.AddAsync(command, cancellationToken);
        return command;
    }

    // ✅ UPDATE: Domain already updated in Service
    public Task<bool> UpdateAsync(Command command, CancellationToken cancellationToken = default)
    {
        // EF Core already tracking entity
        return Task.FromResult(true);
    }

    // ✅ DELETE: only remove (no SaveChanges)
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Commands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
            return false;

        dbContext.Commands.Remove(existing);
        return true;
    }
}