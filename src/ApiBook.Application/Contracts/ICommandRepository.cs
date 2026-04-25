using ApiBook.Domain.Entities;

namespace ApiBook.Application.Contracts;

public interface ICommandRepository
{
    Task<IReadOnlyList<Command>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Command>> GetByPlatformIdAsync(int platformId, CancellationToken cancellationToken = default);
    Task<Command?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Command> AddAsync(Command command, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Command command, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
