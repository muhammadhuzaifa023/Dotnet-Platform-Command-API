using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using ApiBook.Domain.Entities;

namespace ApiBook.Application.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlatformService(IPlatformRepository platformRepository, IUnitOfWork unitOfWork)
    {
        _platformRepository = platformRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PlatformReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var platforms = await _platformRepository.GetAllAsync(cancellationToken);
        return platforms.Select(Map).ToList();
    }

    public async Task<PlatformReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var platform = await _platformRepository.GetByIdAsync(id, cancellationToken);
        return platform is null ? null : Map(platform);
    }

    // ✅ CREATE
    public async Task<PlatformReadDto?> CreateAsync(PlatformCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Platform(dto.Name, dto.Publisher);

        var created = await _platformRepository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MUST

        return Map(created);
    }

    // ✅ UPDATE
    public async Task<bool> UpdateAsync(int id, PlatformUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _platformRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return false;

        existing.Update(dto.Name, dto.Publisher);

        await _platformRepository.UpdateAsync(existing, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MUST

        return true;
    }

    // ✅ DELETE (Soft Delete)
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _platformRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return false;

        existing.MarkAsDeleted();

        await _platformRepository.UpdateAsync(existing, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MOST IMPORTANT

        return true;
    }

    private static PlatformReadDto Map(Platform p) =>
        new(p.Id, p.Name, p.Publisher);
}