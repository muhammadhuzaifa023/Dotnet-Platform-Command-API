using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public class AgentOrchestrator(
    IPlatformSearchAgent platformSearchAgent,
    ICommandRecommendationAgent commandRecommendationAgent) : IAgentOrchestrator
{
    public async Task<AgentRecommendResponseDto> RecommendCommandsAsync(
        AgentRecommendRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var steps = new List<string>
        {
            "Received user query.",
            "Resolving target platform."
        };

        var platformResult = await platformSearchAgent.ResolveAsync(request.Query, request.PlatformId, cancellationToken);
        steps.Add(platformResult.Reason);

        if (platformResult.Platform is null)
        {
            steps.Add("No platform resolved; returning empty recommendations.");
            return new AgentRecommendResponseDto(
                request.Query,
                null,
                null,
                platformResult.Confidence,
                [],
                steps);
        }

        steps.Add($"Fetching and ranking commands for {platformResult.Platform.Name}.");
        var suggestions = await commandRecommendationAgent.RecommendAsync(
            platformResult.Platform,
            request.Query,
            request.MaxResults,
            cancellationToken);

        steps.Add($"Generated {suggestions.Count} recommendations.");
        return new AgentRecommendResponseDto(
            request.Query,
            platformResult.Platform.Name,
            platformResult.Platform.Id,
            platformResult.Confidence,
            suggestions,
            steps);
    }
}
