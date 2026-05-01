using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using ApiBook.Domain.Entities;

namespace ApiBook.Application.Services;

public class CommandService : ICommandService
{
    private readonly ICommandRepository _commandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CommandService(ICommandRepository commandRepository, IUnitOfWork unitOfWork)
    {
        _commandRepository = commandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CommandReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var commands = await _commandRepository.GetAllAsync(cancellationToken);
        return commands.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<CommandReadDto>> GetByPlatformIdAsync(int platformId, CancellationToken cancellationToken = default)
    {
        var commands = await _commandRepository.GetByPlatformIdAsync(platformId, cancellationToken);
        return commands.Select(Map).ToList();
    }

    public async Task<CommandReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var command = await _commandRepository.GetByIdAsync(id, cancellationToken);
        return command is null ? null : Map(command);
    }

    // ✅ CREATE
    public async Task<CommandReadDto?> CreateAsync(CommandCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Command(dto.HowTo, dto.CommandLine, dto.PlatformId);

        var created = await _commandRepository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MUST

        return Map(created);
    }

    // ✅ UPDATE
    public async Task<bool> UpdateAsync(int id, CommandUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _commandRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return false;

        existing.Update(dto.HowTo, dto.CommandLine, dto.PlatformId);

        await _commandRepository.UpdateAsync(existing, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MUST

        return true;
    }

    // ✅ DELETE
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _commandRepository.DeleteAsync(id, cancellationToken);

        if (!result)
            return false;

        await _unitOfWork.SaveChangesAsync(cancellationToken); // 🔥 MOST IMPORTANT

        return true;
    }

    private static CommandReadDto Map(Command command) =>
        new(command.Id, command.HowTo, command.CommandLine, command.PlatformId);
}