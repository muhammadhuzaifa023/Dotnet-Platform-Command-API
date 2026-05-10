namespace ApiBook.Application.DTOs;

public sealed record AgentRecommendRequestDto(
    string Query,
    int? PlatformId = null,
    int MaxResults = 5);
