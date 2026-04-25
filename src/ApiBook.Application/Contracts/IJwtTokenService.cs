namespace ApiBook.Application.Contracts;

public interface IJwtTokenService
{
    string GenerateToken(string subject);
}
