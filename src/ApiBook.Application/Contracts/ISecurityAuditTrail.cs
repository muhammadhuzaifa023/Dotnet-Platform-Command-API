using ApiBook.Application.DTOs;

namespace ApiBook.Application.Contracts;

/// <summary>
/// Phase 2: observability + optional persistence for medium/high risk; does not block requests.
/// </summary>
public interface ISecurityAuditTrail
{
    Task AppendAsync(
        SecurityAnalysisResult analysis,
        SecurityRequestSnapshot request,
        CancellationToken cancellationToken = default);
}
