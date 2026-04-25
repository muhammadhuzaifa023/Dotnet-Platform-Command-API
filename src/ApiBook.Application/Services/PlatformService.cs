using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using ApiBook.Domain.Entities;

namespace ApiBook.Application.Services;

public class PlatformService(IPlatformRepository platformRepository) : IPlatformService
{
    public async Task<IReadOnlyList<PlatformReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var platforms = await platformRepository.GetAllAsync(cancellationToken);
        return platforms
            .Select(Map)
            .ToList();
    }

    public async Task<PlatformReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var platform = await platformRepository.GetByIdAsync(id, cancellationToken);
        return platform is null ? null : Map(platform);
    }

    // ✅ Nayi methods
    public async Task<PlatformReadDto?> CreateAsync(PlatformCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Platform
        {
            Name = dto.Name,
            Publisher = dto.Publisher
        };
        var created = await platformRepository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, PlatformUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Platform
        {
            Id = id,
            Name = dto.Name,
            Publisher = dto.Publisher
        };
        return await platformRepository.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        platformRepository.DeleteAsync(id, cancellationToken);

    // Map helper
    private static PlatformReadDto Map(Platform p) =>
        new(p.Id, p.Name, p.Publisher);

}
