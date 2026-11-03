namespace Application.Common;

public record IdempotencySnapshot(int StatusCode, string ResponseBody);

public interface IIdempotencyService
{
    Task<IdempotencySnapshot?> GetAsync(
        string key,
        Guid userId,
        string endpoint,
        CancellationToken cancellationToken = default);

    Task<bool> TryStoreAsync(
        string key,
        Guid userId,
        string endpoint,
        int statusCode,
        string responseBody,
        CancellationToken cancellationToken = default);
}
