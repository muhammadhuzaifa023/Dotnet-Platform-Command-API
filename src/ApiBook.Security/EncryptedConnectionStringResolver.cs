using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApiBook.Security;

public interface IEncryptedConnectionStringResolver
{
    string Resolve(IConfiguration configuration, string connectionName);
    string EncryptForCurrentUser(string plainTextConnectionString);
}

public class EncryptedConnectionStringResolver : IEncryptedConnectionStringResolver
{
    private const string Prefix = "enc:";
    private const string EncryptionKeyEnvName = "APIBOOK_CONN_ENCRYPTION_KEY";

    public string Resolve(IConfiguration configuration, string connectionName)
    {
        var plainOverride = configuration[$"ConnectionStrings:{connectionName}Plain"];
        if (!string.IsNullOrWhiteSpace(plainOverride))
        {
            return plainOverride;
        }

        var configuredValue = configuration.GetConnectionString(connectionName);

        if (string.IsNullOrWhiteSpace(configuredValue))
        {
            throw new InvalidOperationException($"Missing connection string '{connectionName}'.");
        }

        return configuredValue.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)
            ? Decrypt(configuredValue[Prefix.Length..])
            : configuredValue;
    }

    public string EncryptForCurrentUser(string plainTextConnectionString)
    {
        if (string.IsNullOrWhiteSpace(plainTextConnectionString))
        {
            throw new ArgumentException("Connection string cannot be empty.", nameof(plainTextConnectionString));
        }

        var key = BuildAesKeyFromEnvironment();
        var iv = RandomNumberGenerator.GetBytes(16);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var plainBytes = Encoding.UTF8.GetBytes(plainTextConnectionString);
        using var encryptor = aes.CreateEncryptor();
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var payload = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, payload, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, payload, iv.Length, encryptedBytes.Length);

        return $"{Prefix}{Convert.ToBase64String(payload)}";
    }

    private static string Decrypt(string encryptedValue)
    {
        try
        {
            var key = BuildAesKeyFromEnvironment();
            var payload = Convert.FromBase64String(encryptedValue);
            if (payload.Length <= 16)
            {
                throw new InvalidOperationException("Encrypted payload is too short.");
            }

            var iv = payload[..16];
            var cipherBytes = payload[16..];

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Encrypted connection string has invalid Base64 format.", ex);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Could not decrypt connection string for current user profile.", ex);
        }
    }

    private static byte[] BuildAesKeyFromEnvironment()
    {
        var rawKey = Environment.GetEnvironmentVariable(EncryptionKeyEnvName);
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            throw new InvalidOperationException(
                $"Encryption key missing. Set '{EncryptionKeyEnvName}' environment variable.");
        }

        return SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
    }
}
