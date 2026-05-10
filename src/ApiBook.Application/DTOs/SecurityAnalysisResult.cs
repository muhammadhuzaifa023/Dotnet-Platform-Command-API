using ApiBook.Application.Security;

namespace ApiBook.Application.DTOs;

public sealed record SecurityAnalysisResult(
    int RiskScore,
    SecurityRiskLevel Level,
    IReadOnlyList<string> Reasons);
