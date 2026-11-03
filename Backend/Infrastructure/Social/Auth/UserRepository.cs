using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Social.Auth;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(SocialDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedUsersAsync(
        int page,
        int pageSize,
        string? search = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u => EF.Functions.ILike(u.Email, $"%{s}%") || EF.Functions.ILike(u.DisplayName, $"%{s}%"));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(u => u.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
