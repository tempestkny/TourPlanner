# API Quality And Non-Functional Backend Behavior

## Goal

The backend uses simple ASP.NET Core middleware and framework logging to handle technical cross-cutting concerns consistently.

The implementation follows the lecture approach:

```text
HTTP Request -> Middleware Pipeline -> Controller -> Service -> Repository -> HTTP Response
```

Controllers remain focused on HTTP input/output. Business logic stays in services.

## Global Exception Handling

Unhandled exceptions are handled by `GlobalExceptionMiddleware`.

The middleware:

- catches unexpected exceptions
- logs the exception with `ILogger<GlobalExceptionMiddleware>`
- returns `500 Internal Server Error`
- returns a `ProblemDetails` response
- avoids exposing stack traces to clients

Example response shape:

```json
{
  "type": "about:blank",
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "The server could not process the request.",
  "instance": "/api/example",
  "traceId": "..."
}
```

## Correlation IDs

Every request receives a correlation ID through `CorrelationIdMiddleware`.

The middleware:

- reads `X-Correlation-ID` if the client sends it
- creates a new ID if none exists
- adds `X-Correlation-ID` to the response headers
- adds the correlation ID to the logging scope

This makes it easier to connect a frontend error, an HTTP response and backend log messages.

## ProblemDetails Responses

The backend uses ASP.NET Core `ProblemDetails` for error responses where a custom error body is needed.

Expected errors use normal REST status codes:

- `400 Bad Request` for invalid request data
- `401 Unauthorized` for missing or invalid authentication
- `404 Not Found` for missing resources
- `409 Conflict` for registration conflicts
- `500 Internal Server Error` for unexpected server errors

Unexpected errors are handled by the global exception middleware.

## Structured Logging

The backend uses `ILogger<T>` from ASP.NET Core dependency injection.

Logging follows the lecture recommendations:

- use framework logging
- use parameterized log messages
- log exceptions at the boundary
- do not log passwords
- do not log JWT tokens
- do not log API keys or secrets

Example:

```csharp
logger.LogInformation("Imported {TourCount} tours for user {UserId}", importedTours, userId);
```

## Health Check

The backend exposes a simple health check endpoint:

```text
GET /health
```

The endpoint is public and returns `Healthy` when the API is running.

## Configuration

Runtime configuration is read through ASP.NET Core configuration.

Important values:

```text
ConnectionStrings__TourPlannerDb
Jwt__Issuer
Jwt__Audience
Jwt__Secret
Jwt__ExpirationMinutes
OpenRouteService__ApiKey
```

`JwtOptions` and `OpenRouteServiceOptions` use the Options Pattern.

## Manual Verification

### Health Check

Start the API and open:

```text
http://localhost:5020/health
```

Expected response:

```text
Healthy
```

### Correlation ID

Call any endpoint and inspect the response headers.

Expected header:

```text
X-Correlation-ID: ...
```

If a request sends `X-Correlation-ID`, the same value should be returned.

### ProblemDetails

Trigger a known invalid request, for example an invalid import payload.

Expected:

```text
400 Bad Request
Content-Type: application/problem+json
```

### Logging

Run the API from the terminal and perform login, import/export or TourLog operations.

Expected:

- log messages appear in the backend console
- logs use structured message templates
- logs do not contain passwords, JWT tokens or secrets

## Related Patterns

- Middleware Pattern
- Dependency Injection
- Options Pattern
- Service Layer Pattern
- Repository Pattern
