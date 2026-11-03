using Application.Common;

namespace API.Extensions;

public static class MigrationAndSeedExtensions
{
    /// <summary>
    /// Tự động kiểm tra, đồng bộ schema migration và nạp bộ dữ liệu mẫu khởi động
    /// </summary>
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var initializer = services.GetRequiredService<IDatabaseInitializer>();
            await initializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Lỗi nghiêm trọng khi áp dụng database migrations hoặc seed data khi khởi động: {Message}", ex.Message);
            throw;
        }
    }
}
