namespace ApiBook.Application.DTOs;

/// <summary>
/// Request data needed for heuristic risk scoring (no ASP.NET Core dependency in Application).
/// </summary>
public sealed record SecurityRequestSnapshot(
    string Method,
    string Path,
    string? QueryString,
    string? RemoteIp,
    string? UserAgent,
    bool IsAuthenticated,
    string? UserName);
