using System.Net;
using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;

namespace ApiBook.Application.Security;

public class RiskScoreEngine : IRiskScoreEngine
{
    public SecurityAnalysisResult Evaluate(SecurityRequestSnapshot request)
    {
        var reasons = new List<string>();
        var score = 0;

        var path = request.Path ?? string.Empty;
        var queryRaw = request.QueryString ?? string.Empty;
        // HttpContext.QueryString.Value is usually percent-encoded; decode so heuristics see real payload.
        var queryDecoded = DecodeQueryForAnalysis(queryRaw);
        var pathDecoded = TryUrlDecodePath(path);
        var combined = $"{pathDecoded} {queryRaw} {queryDecoded}".ToLowerInvariant();

        if (MatchesSqlInjectionSignals(combined))
        {
            score += 45;
            reasons.Add("Suspicious SQL-injection-like pattern in path or query.");
        }

        if (path.Contains("%2e%2e", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("..", StringComparison.Ordinal) ||
            pathDecoded.Contains("..", StringComparison.Ordinal))
        {
            score += 35;
            reasons.Add("Path traversal pattern detected.");
        }

        if (combined.Contains("<script", StringComparison.Ordinal) ||
            combined.Contains("javascript:", StringComparison.Ordinal))
        {
            score += 35;
            reasons.Add("Possible XSS payload in path or query.");
        }

        if (MatchesSensitivePaths(path) || MatchesSensitivePaths(pathDecoded))
        {
            score += 30;
            reasons.Add("Request targets a sensitive probe path (e.g. .env, .git).");
        }

        if (!string.IsNullOrEmpty(request.UserAgent) &&
            MatchesScannerUserAgent(request.UserAgent))
        {
            score += 25;
            reasons.Add("User-Agent matches common security scanner fingerprint.");
        }

        if (queryRaw.Length > 2048 || queryDecoded.Length > 2048)
        {
            score += 15;
            reasons.Add("Unusually long query string.");
        }

        score = Math.Clamp(score, 0, 100);

        var level = score switch
        {
            < 20 => SecurityRiskLevel.Low,
            <= 70 => SecurityRiskLevel.Medium,
            _ => SecurityRiskLevel.High
        };

        if (reasons.Count == 0)
            reasons.Add("No suspicious heuristics triggered.");

        return new SecurityAnalysisResult(score, level, reasons);
    }

    /// <summary>Strip leading ? and URL-decode (handles + as space).</summary>
    private static string DecodeQueryForAnalysis(string query)
    {
        if (string.IsNullOrEmpty(query))
            return string.Empty;

        var trimmed = query.StartsWith("?", StringComparison.Ordinal) ? query[1..] : query;
        try
        {
            return WebUtility.UrlDecode(trimmed) ?? trimmed;
        }
        catch
        {
            return trimmed;
        }
    }

    private static string TryUrlDecodePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        try
        {
            return Uri.UnescapeDataString(path);
        }
        catch
        {
            return path;
        }
    }

    private static bool MatchesSqlInjectionSignals(string combined)
    {
        string[] patterns =
        [
            "' or ", " or 1=", "union select", "; drop ", ";--", "1=1",
            "exec(", "xp_cmdshell", "/etc/passwd"
        ];

        return patterns.Any(p => combined.Contains(p, StringComparison.Ordinal));
    }

    private static bool MatchesSensitivePaths(string path)
    {
        string[] segments =
        [
            "/.env", "/.git",             "wp-admin", "phpmyadmin", "actuator",
            "/server-status"
        ];

        var lower = path.ToLowerInvariant();
        return segments.Any(s => lower.Contains(s, StringComparison.Ordinal));
    }

    private static bool MatchesScannerUserAgent(string userAgent)
    {
        var lower = userAgent.ToLowerInvariant();
        string[] markers = ["sqlmap", "nikto", "nuclei", "masscan", "zgrab", "gobuster"];
        return markers.Any(m => lower.Contains(m, StringComparison.Ordinal));
    }
}
