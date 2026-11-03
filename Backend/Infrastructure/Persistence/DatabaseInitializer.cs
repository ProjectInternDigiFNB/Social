using Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly SocialDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(SocialDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Đang kiểm tra và áp dụng Database Migrations...");

        // Tự động đồng bộ lịch sử migration cho PostEmbeddings nếu bảng đã được tạo thủ công trước đó
        try
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'PostEmbeddings') THEN
                        INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                        VALUES ('20260929044949_AddPostEmbeddings', '8.0.8')
                        ON CONFLICT DO NOTHING;
                    END IF;
                END $$;", cancellationToken);
        }
        catch (Exception migrationSyncEx)
        {
            _logger.LogWarning("Không thể đồng bộ bảng migration trước: {Message}", migrationSyncEx.Message);
        }

        await _context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Database Migrations đã được áp dụng thành công.");
    }
}
