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

    public async Task<Command?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Commands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Command> AddAsync(Command command, CancellationToken cancellationToken = default)
    {
        dbContext.Commands.Add(command);
        await dbContext.SaveChangesAsync(cancellationToken);
        return command;
    }

    public async Task<bool> UpdateAsync(Command command, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Commands.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.HowTo = command.HowTo;
        existing.CommandLine = command.CommandLine;
        existing.PlatformId = command.PlatformId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Commands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Commands.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
