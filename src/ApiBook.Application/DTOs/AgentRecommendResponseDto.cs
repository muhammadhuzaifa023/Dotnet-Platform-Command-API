namespace ApiBook.Application.DTOs;

public sealed record AgentRecommendResponseDto(
    string Query,
    string? SelectedPlatform,
    int? SelectedPlatformId,
    double Confidence,
    IReadOnlyList<AgentCommandSuggestionDto> Suggestions,
    IReadOnlyList<string> Steps);

public sealed record AgentCommandSuggestionDto(
    string HowTo,
    string CommandLine,
    string PlatformName,
    int PlatformId,
    double Score);
