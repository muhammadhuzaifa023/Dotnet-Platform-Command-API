using ApiBook.Application.Contracts;
using ApiBook.Domain.Entities;
using ApiBook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiBook.Infrastructure.Repositories;

public class SecurityAuditLogRepository(AppDbContext dbContext) : ISecurityAuditLogRepository
{
    public async Task AddAsync(SecurityAuditLog entry, CancellationToken cancellationToken = default)
    {
        await dbContext.SecurityAuditLogs.AddAsync(entry, cancellationToken);
    }
}
