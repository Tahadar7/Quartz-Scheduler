using QuartzScheduler.API.Options;
using System.Text;
using System.Text.Json;

namespace QuartzScheduler.API.Services;

public class HttpApiService(HttpClient httpClient) : IHttpApiService
{
    public async Task<HttpApiCallResult> CallAsync(
        string url,
        string httpMethod,
        string? headersJson,
        string? requestBody,
        CancellationToken ct = default)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = new HttpMethod(httpMethod.ToUpperInvariant())
        };

        if (!string.IsNullOrWhiteSpace(requestBody))
        {
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

        ApplyHeaders(request, headersJson);

        using var response = await httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        return new HttpApiCallResult
        {
            StatusCode = (int)response.StatusCode,
            ResponseBody = body
        };
    }

    // headers are validated as a JSON object at the request layer
    // here they're deserialized into actual request headers
    private static void ApplyHeaders(HttpRequestMessage request, string? headersJson)
    {
        if (string.IsNullOrWhiteSpace(headersJson))
        {
            return;
        }

        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson);
        if (headers is null)
            return;

        foreach (var (key, value) in headers)
        {
            if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase) && request.Content is not null)
            {
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(value);
            }
            else
            {
                request.Headers.TryAddWithoutValidation(key, value);
            }
        }
    }
}