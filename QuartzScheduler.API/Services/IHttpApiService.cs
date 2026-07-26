using QuartzScheduler.API.Options;
namespace QuartzScheduler.API.Services;

public interface IHttpApiService
{
    Task<HttpApiCallResult> CallAsync(
        string url,
        string httpMethod,
        string? headersJson,
        string? requestBody,
        CancellationToken ct = default);
}