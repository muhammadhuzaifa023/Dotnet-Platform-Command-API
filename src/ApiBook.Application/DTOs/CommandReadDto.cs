namespace ApiBook.Application.DTOs;

public sealed record CommandReadDto(int Id, string HowTo, string CommandLine, int PlatformId);
