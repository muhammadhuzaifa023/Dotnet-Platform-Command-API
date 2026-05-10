using ApiBook.Application.Contracts;
using ApiBook.Application.Services;
using ApiBook.Application.Agents;
using ApiBook.Application.Security;
using ApiBook.Infrastructure.Persistence;
using ApiBook.Infrastructure.Repositories;
using ApiBook.Infrastructure.Security;
using ApiBook.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiBook.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<ICommandService, CommandService>();
        services.AddScoped<IPlatformSearchAgent, PlatformSearchAgent>();
        services.AddScoped<ICommandRecommendationAgent, CommandRecommendationAgent>();
        services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecurityAuditOptions>(configuration.GetSection("SecurityAudit"));
        services.AddSingleton<IRiskScoreEngine, RiskScoreEngine>();
        services.AddScoped<ISecurityAuditTrail, SecurityAuditTrailService>();
        services.AddScoped<ISecurityAuditLogRepository, SecurityAuditLogRepository>();

        services.AddSingleton<IEncryptedConnectionStringResolver, EncryptedConnectionStringResolver>();

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var resolver = provider.GetRequiredService<IEncryptedConnectionStringResolver>();
            var connectionString = resolver.Resolve(configuration, "Postgres");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<ICommandRepository, CommandRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IApiKeyValidator, ConfigurationApiKeyValidator>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
