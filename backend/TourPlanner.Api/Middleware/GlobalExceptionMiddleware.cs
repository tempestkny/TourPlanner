using Microsoft.AspNetCore.Mvc;

namespace TourPlanner.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate next; // next middleware in the pipeline
    private readonly ILogger<GlobalExceptionMiddleware> logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // no exception occurred, continue to the handler/controller
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", // Log error with exception details, http method, and path
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw; // If the response already started, rethrow the exception let the server handle it
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            { // standardized error response for api errors
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "The server could not process the request.",
                Instance = context.Request.Path
            };
            problemDetails.Extensions["traceId"] = context.TraceIdentifier; // add traceid to problem details for correlation and debugging

            await context.Response.WriteAsJsonAsync(problemDetails); // json response
        }
    }
}
