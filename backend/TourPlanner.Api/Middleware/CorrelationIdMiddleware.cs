namespace TourPlanner.Api.Middleware;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID"; // header name

    private readonly RequestDelegate next; // next middleware in the pipeline
    private readonly ILogger<CorrelationIdMiddleware> logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    { // for each request, check if a correlation id is present in the request headers, if not generate a new one and add it to the response headers
        var correlationId = GetOrCreateCorrelationId(context);

        context.Response.OnStarting(() =>
        { // before sending, add correlation id to header
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object>
        { // structured logging scope, add correlation id for all log entries in this request
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var correlationId)
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
