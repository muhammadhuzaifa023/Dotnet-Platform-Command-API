using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApiBook.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration, IJwtTokenService jwtTokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("token")]
    public IActionResult IssueToken([FromBody] AuthLoginRequestDto request)
    {
        var expectedUsername = configuration["Security:AuthUser:Username"];
        var expectedPassword = configuration["Security:AuthUser:Password"];
        if (string.IsNullOrWhiteSpace(expectedUsername) || string.IsNullOrWhiteSpace(expectedPassword))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Auth credentials not configured." });
        }

        if (!string.Equals(request.Username, expectedUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var token = jwtTokenService.GenerateToken(request.Username);
        var expiryMinutes = int.TryParse(configuration["Security:Jwt:ExpiryMinutes"], out var value) ? value : 60;
        var response = new AuthTokenResponseDto(token, DateTime.UtcNow.AddMinutes(expiryMinutes));
        return Ok(response);
    }
}
