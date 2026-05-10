using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public class CommandRecommendationAgent(ICommandService commandService) : ICommandRecommendationAgent
{
    public async Task<IReadOnlyList<AgentCommandSuggestionDto>> RecommendAsync(
        PlatformReadDto platform,
        string query,
        int maxResults,
        CancellationToken cancellationToken = default)
    {
        var commands = await commandService.GetByPlatformIdAsync(platform.Id, cancellationToken);
        if (commands.Count == 0)
        {
            return [];
        }

        var normalized = query.Trim().ToLowerInvariant();
        var scored = commands
            .Select(c =>
            {
                var score = CalculateScore(normalized, c.HowTo, c.CommandLine);
                return new AgentCommandSuggestionDto(c.HowTo, c.CommandLine, platform.Name, platform.Id, score);
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.HowTo)
            .Take(Math.Clamp(maxResults, 1, 10))
            .ToList();

        return scored;
    }

    private static double CalculateScore(string query, string howTo, string commandLine)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return 0.40;
        }

        var score = 0.10;
        var loweredHowTo = howTo.ToLowerInvariant();
        var loweredCommand = commandLine.ToLowerInvariant();

        if (loweredHowTo.Contains(query, StringComparison.Ordinal) ||
            loweredCommand.Contains(query, StringComparison.Ordinal))
        {
            score += 0.70;
        }

        var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var token in tokens)
        {
            if (token.Length < 3)
            {
                continue;
            }

            if (loweredHowTo.Contains(token, StringComparison.Ordinal))
            {
                score += 0.08;
            }

            if (loweredCommand.Contains(token, StringComparison.Ordinal))
            {
                score += 0.10;
            }
        }

        return Math.Min(score, 1.0);
    }
}
