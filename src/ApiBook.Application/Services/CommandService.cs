using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using ApiBook.Domain.Entities;

namespace ApiBook.Application.Services;

public class CommandService(ICommandRepository commandRepository) : ICommandService
{
    public async Task<IReadOnlyList<CommandReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var commands = await commandRepository.GetAllAsync(cancellationToken);
        return commands.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<CommandReadDto>> GetByPlatformIdAsync(int platformId, CancellationToken cancellationToken = default)
    {
        var commands = await commandRepository.GetByPlatformIdAsync(platformId, cancellationToken);
        return commands.Select(Map).ToList();
    }

    public async Task<CommandReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var command = await commandRepository.GetByIdAsync(id, cancellationToken);
        return command is null ? null : Map(command);
    }

    public async Task<CommandReadDto?> CreateAsync(CommandCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            HowTo = dto.HowTo,
            CommandLine = dto.CommandLine,
            PlatformId = dto.PlatformId
        };

        var created = await commandRepository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, CommandUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            Id = id,
            HowTo = dto.HowTo,
            CommandLine = dto.CommandLine,
            PlatformId = dto.PlatformId
        };

        return await commandRepository.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        commandRepository.DeleteAsync(id, cancellationToken);

    private static CommandReadDto Map(Command command) =>
        new(command.Id, command.HowTo, command.CommandLine, command.PlatformId);
}
