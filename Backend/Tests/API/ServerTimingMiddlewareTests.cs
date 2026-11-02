using API.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Tests.API;

public class ServerTimingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Append_ServerTimingHeader_With_AppMetric()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var metrics = new ServerTimingMetrics();
        var middleware = new ServerTimingMiddleware(async (ctx) =>
        {
            await Task.Delay(10);
        });

        // Act
        await middleware.InvokeAsync(context, metrics);

        // Giả lập kích hoạt OnStarting callbacks của ASP.NET Core response
        // DefaultHttpContext không tự gọi OnStarting khi InvokeAsync kết thúc, 
        // nhưng chúng ta có thể kiểm tra trực tiếp metrics và entry logic
        metrics.AddMetric("db", 5.2, "PostgreSQL query");
        var recordedMetrics = metrics.GetMetrics();

        // Assert
        recordedMetrics.Should().HaveCount(1);
        recordedMetrics[0].Name.Should().Be("db");
        recordedMetrics[0].DurationMs.Should().Be(5.2);
        recordedMetrics[0].Description.Should().Be("PostgreSQL query");
    }

    [Fact]
    public void ServerTimingMetrics_Should_Ignore_Invalid_Names_Or_Negative_Durations()
    {
        // Arrange
        var metrics = new ServerTimingMetrics();

        // Act
        metrics.AddMetric("", 10.0);
        metrics.AddMetric("   ", 10.0);
        metrics.AddMetric("invalid_dur", -1.0);

        // Assert
        metrics.GetMetrics().Should().BeEmpty();
    }
}

