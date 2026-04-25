using ApiBook.Domain.Entities;

namespace ApiBook.Application.Contracts;

public interface IPlatformRepository
{
    Task<IReadOnlyList<Platform>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Platform?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Platform> AddAsync(Platform platform, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Platform platform, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
