using ApiBook.Application.Agents;
using ApiBook.Application.Common;
using ApiBook.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiBook.Presentation.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController(IAgentOrchestrator orchestrator) : ControllerBase
{
    [HttpPost("commands/recommend")]
    public async Task<IActionResult> RecommendCommands(
        [FromBody] AgentRecommendRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(ApiResponse<object>.Fail("Query is required."));
        }

        var response = await orchestrator.RecommendCommandsAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(response, "Recommendations generated successfully."));
    }
}
