using ApiBook.Application.DTOs;

namespace ApiBook.Application.Contracts;

public interface ICommandService
{
    Task<IReadOnlyList<CommandReadDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommandReadDto>> GetByPlatformIdAsync(int platformId, CancellationToken cancellationToken = default);
    Task<CommandReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CommandReadDto?> CreateAsync(CommandCreateDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CommandUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
