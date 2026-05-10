using ApiBook.Application.DTOs;

namespace ApiBook.Application.Agents;

public sealed record PlatformResolutionResult(
    PlatformReadDto? Platform,
    double Confidence,
    string Reason);
