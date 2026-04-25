using ApiBook.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace ApiBook.Infrastructure.Security;

public class ConfigurationApiKeyValidator(IConfiguration configuration) : IApiKeyValidator
{
    public bool IsValid(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        var configuredApiKey = configuration["Security:ApiKey"];
        return !string.IsNullOrWhiteSpace(configuredApiKey) &&
               string.Equals(configuredApiKey, apiKey, StringComparison.Ordinal);
    }
}
