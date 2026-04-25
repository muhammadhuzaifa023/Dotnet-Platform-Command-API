namespace ApiBook.Application.DTOs;

public sealed record CommandCreateDto(string HowTo, string CommandLine, int PlatformId);
