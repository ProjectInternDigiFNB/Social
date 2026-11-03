using Application.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly SocialDbContext _dbContext;
    private readonly ILogger<IdempotencyService> _logger;

    public IdempotencyService(SocialDbContext dbContext, ILogger<IdempotencyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IdempotencySnapshot?> GetAsync(
        string key,
        Guid userId,
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.Key == key && r.UserId == userId && r.Endpoint == endpoint,
                cancellationToken);

        return existing == null
            ? null
            : new IdempotencySnapshot(existing.StatusCode, existing.ResponseBody);
    }

    public async Task<bool> TryStoreAsync(
        string key,
        Guid userId,
        string endpoint,
        int statusCode,
        string responseBody,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var record = new IdempotencyRecord
            {
                Key = key,
                UserId = userId,
                Endpoint = endpoint,
                StatusCode = statusCode,
                ResponseBody = responseBody
            };

            _dbContext.IdempotencyRecords.Add(record);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            // Trường hợp 2 request đồng thời cùng Key: Unique index ngăn chặn bản ghi thứ 2
            _logger.LogWarning(ex, "Xung đột ghi IdempotencyRecord đồng thời cho Key='{Key}'. Bỏ qua lưu đúp.", key);
            return false;
        }
    }
}
