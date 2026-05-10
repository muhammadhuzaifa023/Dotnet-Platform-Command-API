using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public interface IPlatformSearchAgent
{
    Task<PlatformResolutionResult> ResolveAsync(
        string query,
        int? platformId,
        CancellationToken cancellationToken = default);
}
