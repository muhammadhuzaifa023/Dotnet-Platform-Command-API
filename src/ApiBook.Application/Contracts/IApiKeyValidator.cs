namespace ApiBook.Application.Contracts;

public interface IApiKeyValidator
{
    bool IsValid(string? apiKey);
}
