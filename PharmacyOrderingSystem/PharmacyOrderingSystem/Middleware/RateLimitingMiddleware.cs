
namespace PharmacyOrderingSystem.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static Dictionary<string, (int count, DateTime time)> _requests = new();

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (_requests.ContainsKey(ip))
        {
            var (count, time) = _requests[ip];

            if ((DateTime.UtcNow - time).TotalSeconds < 60)
            {
                if (count > 10)
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync("Too many requests");
                    return;
                }

                _requests[ip] = (count + 1, time);
            }
            else
            {
                _requests[ip] = (1, DateTime.UtcNow);
            }
        }
        else
        {
            _requests[ip] = (1, DateTime.UtcNow);
        }

        await _next(context);
    }
}