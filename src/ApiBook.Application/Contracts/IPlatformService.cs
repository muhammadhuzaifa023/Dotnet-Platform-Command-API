using ApiBook.Application.DTOs;

namespace ApiBook.Application.Contracts;

public interface IPlatformService
{
    Task<IReadOnlyList<PlatformReadDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PlatformReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PlatformReadDto?> CreateAsync(PlatformCreateDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PlatformUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
