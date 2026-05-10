using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public interface ICommandRecommendationAgent
{
    Task<IReadOnlyList<AgentCommandSuggestionDto>> RecommendAsync(
        PlatformReadDto platform,
        string query,
        int maxResults,
        CancellationToken cancellationToken = default);
}
