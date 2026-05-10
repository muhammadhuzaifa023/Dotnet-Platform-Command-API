namespace ApiBook.Domain.Entities;

public class SecurityAuditLog
{
    public long Id { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }
    public string RiskLevel { get; private set; } = string.Empty;
    public int RiskScore { get; private set; }
    public string HttpMethod { get; private set; } = string.Empty;
    public string Path { get; private set; } = string.Empty;
    public string? QueryString { get; private set; }
    public string? RemoteIp { get; private set; }
    public string? UserAgent { get; private set; }
    public string ReasonsSummary { get; private set; } = string.Empty;

    private SecurityAuditLog()
    {
    }

    public SecurityAuditLog(
        string riskLevel,
        int riskScore,
        string httpMethod,
        string path,
        string? queryString,
        string? remoteIp,
        string? userAgent,
        string reasonsSummary)
    {
        if (string.IsNullOrWhiteSpace(riskLevel))
            throw new ArgumentException("Risk level is required", nameof(riskLevel));
        if (string.IsNullOrWhiteSpace(httpMethod))
            throw new ArgumentException("HTTP method is required", nameof(httpMethod));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is required", nameof(path));

        RecordedAtUtc = DateTime.UtcNow;
        RiskLevel = riskLevel;
        RiskScore = riskScore;
        HttpMethod = httpMethod.Trim();
        Path = path.Trim();
        QueryString = Truncate(queryString, 4000);
        RemoteIp = Truncate(remoteIp, 100);
        UserAgent = Truncate(userAgent, 500);
        ReasonsSummary = Truncate(reasonsSummary, 8000) ?? string.Empty;
    }

    private static string? Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Length <= max ? value : value[..max];
    }
}
