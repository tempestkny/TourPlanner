# Statistics Dashboard Flow

## Goal

The statistics dashboard gives an authenticated user a quick overview of their own TourPlanner data.

It shows:

- total Tours
- total TourLogs
- total route distance
- total route time
- average TourLog rating

The implementation follows the project architecture:

```text
Angular UI -> ASP.NET API Controller -> Business Logic Service -> Repository -> EF Core/PostgreSQL
```

Each layer only communicates with the layer directly below it.

## Flow

1. The user logs in and receives a JWT.
2. The Angular shell shows the statistics dashboard as the default right-side view.
3. `StatisticsDashboard` calls `StatisticsService.getStatistics()`.
4. The HTTP interceptor adds the JWT to the request.
5. `StatisticsController` reads the authenticated user id from the JWT.
6. The controller delegates to `IStatisticsService`.
7. `StatisticsService` loads only Tours owned by the authenticated user.
8. For each owned Tour, the service loads related TourLogs.
9. The service calculates the dashboard values and returns `StatisticsResponseDto`.
10. The frontend displays the values through Angular data binding.

## API Endpoint

The endpoint is protected with JWT authentication.

```text
GET /api/statistics
```

## Layer Responsibilities

### UI Layer

`StatisticsDashboard` is responsible for presentation state:

- loading state
- error message
- refresh button
- displaying computed values

The Angular `StatisticsService` performs only HTTP communication with the backend.

### API Layer

`StatisticsController` remains thin. It reads the user id from the JWT:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

The controller does not calculate statistics and does not access repositories directly.

### Business Logic Layer

`StatisticsService` contains the calculation logic:

- count owned Tours
- count TourLogs belonging to owned Tours
- sum route distance from stored route information
- sum route time from stored route information
- calculate average rating
- return zero values when no data exists

### Data Access Layer

The service uses existing repositories:

- `ITourRepository` loads Tours for the authenticated user.
- `ITourLogRepository` loads TourLogs by Tour id.

Repositories stay responsible only for persistence and querying.

## DTO Usage

The frontend and backend do not exchange EF entities directly.

`StatisticsResponseDto` contains:

```text
TotalTours
TotalTourLogs
TotalDistance
TotalTime
AverageRating
```

## Authentication And Ownership

The frontend does not send a user id.

The backend uses the JWT as the source of truth. Only Tours returned for the authenticated user are used for statistics. TourLogs are loaded through those owned Tour ids, so data from other users is not included.

## Error Handling

If the user is not authenticated, the API returns `401 Unauthorized`.

If a legacy Tour has no route information, the statistics service treats distance and time as zero for that Tour. This prevents the dashboard from failing because of older data.

## Tests

Statistics behavior is covered in `StatisticsServiceTests`.

The tests verify:

- empty users receive zero values
- totals are calculated from Tours and TourLogs
- foreign user data is not included
- legacy Tours without route information do not crash the calculation

## Manual Test

1. Start PostgreSQL and the backend.
2. Start the Angular frontend.
3. Register or log in.
4. Create at least one Tour.
5. Add at least one TourLog.
6. Open the dashboard or click `Dashboard`.
7. Verify that totals and average rating are shown.
8. Click `Refresh` and verify the values reload.
