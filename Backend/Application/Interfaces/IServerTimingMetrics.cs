namespace Application.Interfaces;

/// <summary>
/// Giao diện cho phép các dịch vụ đo lường và ghi nhận chi tiết thời gian xử lý (W3C Server-Timing)
/// </summary>
public interface IServerTimingMetrics
{
    void AddMetric(string name, double durationMs, string? description = null);
    IReadOnlyList<ServerTimingEntry> GetMetrics();
}

public readonly record struct ServerTimingEntry(string Name, double DurationMs, string? Description);

