using ApiBook.Domain.Entities;

namespace ApiBook.Application.Contracts;

public interface ISecurityAuditLogRepository
{
    Task AddAsync(SecurityAuditLog entry, CancellationToken cancellationToken = default);
}
