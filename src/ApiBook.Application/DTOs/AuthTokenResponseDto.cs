namespace ApiBook.Application.DTOs;

public sealed record AuthTokenResponseDto(string AccessToken, DateTime ExpiresAtUtc);
