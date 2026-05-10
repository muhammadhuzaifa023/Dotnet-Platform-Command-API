using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public interface IAgentOrchestrator
{
    Task<AgentRecommendResponseDto> RecommendCommandsAsync(
        AgentRecommendRequestDto request,
        CancellationToken cancellationToken = default);
}
