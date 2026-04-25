using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiBook.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiBook.Infrastructure.Security;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateToken(string subject)
    {
        var issuer = configuration["Security:Jwt:Issuer"] ?? throw new InvalidOperationException("Security:Jwt:Issuer missing.");
        var audience = configuration["Security:Jwt:Audience"] ?? throw new InvalidOperationException("Security:Jwt:Audience missing.");
        var signingKey = configuration["Security:Jwt:SigningKey"] ?? throw new InvalidOperationException("Security:Jwt:SigningKey missing.");
        var expiryMinutes = int.TryParse(configuration["Security:Jwt:ExpiryMinutes"], out var value) ? value : 60;

        var now = DateTime.UtcNow;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, subject)],
            notBefore: now,
            expires: now.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
