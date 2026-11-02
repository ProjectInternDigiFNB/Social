using System.Collections.Concurrent;
using System.Diagnostics;
using Application.Interfaces;

namespace API.Extensions;

public sealed class ServerTimingMetrics : IServerTimingMetrics
{
    private readonly ConcurrentBag<ServerTimingEntry> _entries = new();

    public void AddMetric(string name, double durationMs, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name) || durationMs < 0) return;
        _entries.Add(new ServerTimingEntry(name, durationMs, description));
    }

    public IReadOnlyList<ServerTimingEntry> GetMetrics() => _entries.ToArray();
}

/// <summary>
/// Middleware đo lường và phát sinh HTTP Server-Timing Header (W3C Standard)
/// Giúp Chrome DevTools Network hiển thị chính xác biểu đồ thời gian xử lý của backend như Facebook
/// </summary>
public sealed class ServerTimingMiddleware
{
    private readonly RequestDelegate _next;

    public ServerTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServerTimingMetrics timingMetrics)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var totalMs = stopwatch.Elapsed.TotalMilliseconds;

            var metrics = timingMetrics.GetMetrics();
            var parts = new List<string>(metrics.Count + 1);

            // Ghi nhận tổng thời gian xử lý của ứng dụng
            parts.Add($"app;dur={totalMs:F1};desc=\"Total Processing\"");

            // Ghi nhận các metric chi tiết từ cache, db, v.v.
            foreach (var metric in metrics)
            {
                var descPart = string.IsNullOrWhiteSpace(metric.Description)
                    ? string.Empty
                    : $";desc=\"{metric.Description}\"";
                parts.Add($"{metric.Name};dur={metric.DurationMs:F1}{descPart}");
            }

            var headerValue = string.Join(", ", parts);
            if (!context.Response.Headers.ContainsKey("Server-Timing"))
            {
                context.Response.Headers.Append("Server-Timing", headerValue);
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

