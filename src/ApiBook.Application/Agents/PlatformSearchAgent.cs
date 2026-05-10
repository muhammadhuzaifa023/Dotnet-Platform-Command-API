using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public class PlatformSearchAgent(IPlatformService platformService) : IPlatformSearchAgent
{
    public async Task<PlatformResolutionResult> ResolveAsync(
        string query,
        int? platformId,
        CancellationToken cancellationToken = default)
    {
        if (platformId.HasValue)
        {
            var byId = await platformService.GetByIdAsync(platformId.Value, cancellationToken);
            if (byId is not null)
            {
                return new PlatformResolutionResult(byId, 1.0, "Resolved by explicit platformId.");
            }
        }

        var platforms = await platformService.GetAllAsync(cancellationToken);
        if (platforms.Count == 0)
        {
            return new PlatformResolutionResult(null, 0, "No platforms available in database.");
        }

        var normalizedQuery = query.Trim().ToLowerInvariant();
        var exactNameMatch = platforms.FirstOrDefault(p =>
            string.Equals(p.Name, normalizedQuery, StringComparison.OrdinalIgnoreCase));
        if (exactNameMatch is not null)
        {
            return new PlatformResolutionResult(exactNameMatch, 0.95, "Exact platform name match from query.");
        }

        var containsNameMatch = platforms.FirstOrDefault(p =>
            normalizedQuery.Contains(p.Name.ToLowerInvariant(), StringComparison.Ordinal));
        if (containsNameMatch is not null)
        {
            return new PlatformResolutionResult(containsNameMatch, 0.80, "Query contains platform name.");
        }

        var publisherMatch = platforms.FirstOrDefault(p =>
            normalizedQuery.Contains(p.Publisher.ToLowerInvariant(), StringComparison.Ordinal));
        if (publisherMatch is not null)
        {
            return new PlatformResolutionResult(publisherMatch, 0.60, "Resolved by publisher keyword.");
        }

        return new PlatformResolutionResult(platforms[0], 0.35, "No strong match; fallback to first platform.");
    }
}
