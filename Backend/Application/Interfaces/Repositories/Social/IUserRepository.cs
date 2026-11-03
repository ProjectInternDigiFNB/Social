using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedUsersAsync(
        int page,
        int pageSize,
        string? search = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default);
}
