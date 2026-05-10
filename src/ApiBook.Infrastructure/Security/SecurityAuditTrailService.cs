using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using ApiBook.Application.Security;
using ApiBook.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiBook.Infrastructure.Security;

public sealed class SecurityAuditOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>Persist medium/high to database when true.</summary>
    public bool PersistMediumAndHigh { get; set; } = true;
}

public sealed class SecurityAuditTrailService(
    ISecurityAuditLogRepository auditLogRepository,
    IUnitOfWork unitOfWork,
    ILogger<SecurityAuditTrailService> logger,
    IOptions<SecurityAuditOptions> options) : ISecurityAuditTrail
{
    private readonly SecurityAuditOptions _options = options.Value;

    public async Task AppendAsync(
        SecurityAnalysisResult analysis,
        SecurityRequestSnapshot request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return;

        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["Security.RiskScore"] = analysis.RiskScore,
            ["Security.RiskLevel"] = analysis.Level.ToString(),
            ["Security.Path"] = request.Path,
            ["Security.Method"] = request.Method,
            ["Security.RemoteIp"] = request.RemoteIp,
            ["Security.Reasons"] = string.Join("; ", analysis.Reasons)
        }))
        {
            switch (analysis.Level)
            {
                case SecurityRiskLevel.Low:
                    logger.LogDebug("Security audit: low risk.");
                    break;
                case SecurityRiskLevel.Medium:
                    logger.LogWarning("Security audit: medium risk — review recommended.");
                    break;
                case SecurityRiskLevel.High:
                    logger.LogError("Security audit: high risk — immediate review.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (!_options.PersistMediumAndHigh || analysis.Level == SecurityRiskLevel.Low)
            return;

        var summary = string.Join(" | ", analysis.Reasons);
        var entity = new SecurityAuditLog(
            analysis.Level.ToString(),
            analysis.RiskScore,
            request.Method,
            request.Path,
            request.QueryString,
            request.RemoteIp,
            request.UserAgent,
            summary);

        await auditLogRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
