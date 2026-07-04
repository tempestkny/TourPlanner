# TourLog Flow

## Goal

TourLogs allow a user to document completed tour statistics for an existing tour. A TourLog belongs to exactly one Tour and contains date/time, comment, difficulty, total distance, total time and rating.

The implementation follows the project architecture:

```text
Angular UI -> ASP.NET API Controller -> Business Logic Service -> Repository -> EF Core/PostgreSQL
```

Each layer only calls the layer directly below it.

## Flow

1. The user selects a Tour in the Angular UI.
2. The frontend loads TourLogs for the selected Tour through `TourLogService`.
3. The HTTP interceptor adds the JWT to the request.
4. `TourLogController` reads the authenticated user id from the JWT.
5. The controller delegates to `ITourLogService`.
6. `TourLogService` checks that the selected Tour belongs to the authenticated user.
7. `TourLogRepository` reads or writes TourLog entities through EF Core.
8. The backend returns TourLog DTOs, not EF entities.
9. The Angular LogListService updates the local TourLog state.

## API Endpoints

All TourLog endpoints are protected with JWT authentication.

```text
GET    /api/tourlogs/tour/{tourId}
GET    /api/tourlogs/{id}
POST   /api/tourlogs
PUT    /api/tourlogs/{id}
DELETE /api/tourlogs/{id}
```

## Layer Responsibilities

### UI Layer

The Angular TourLog components handle form input, validation messages and view changes.

The Angular `TourLogService` performs HTTP communication with the backend. It maps API timestamp strings to `Date` objects and maps difficulty values between frontend and backend representation.

`LogListService` owns the local TourLog state and calls `TourLogService` for backend operations.

### API Layer

`TourLogController` exposes the TourLog endpoints and remains thin. It reads the user id from the JWT:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

The controller does not contain ownership or persistence logic.

### Business Logic Layer

`TourLogService` contains TourLog business rules:

- verify that the Tour belongs to the authenticated user
- create TourLogs for owned Tours only
- return logs only for owned Tours
- update owned TourLogs only
- delete owned TourLogs only
- map TourLog entities to response DTOs

### Data Access Layer

`TourLogRepository` is responsible for persistence only. It provides async EF Core operations:

- `Create`
- `Read`
- `ReadByTourId`
- `Update`
- `Delete`

The repository does not check ownership and does not contain business logic.

## DTO Usage

The frontend and backend do not exchange EF entities directly.

`CreateTourLogDto` is used for creating logs:

```text
TourId
TimeStamp
Comment
Difficulty
TotalDistance
TotalTime
Rating
```

`UpdateTourLogDto` is used for updating logs:

```text
TimeStamp
Comment
Difficulty
TotalDistance
TotalTime
Rating
```

`TourLogResponseDto` is used for safe responses:

```text
Id
TourId
TimeStamp
Comment
Difficulty
TotalDistance
TotalTime
Rating
```

## Authentication And Ownership

The frontend does not send a user id for TourLog ownership decisions.

The backend uses the validated JWT as the source of truth. The user id is read from `ClaimTypes.NameIdentifier` and passed to `TourLogService`.

`TourLogService` verifies ownership by loading the related Tour and comparing `Tour.UserId` with the authenticated user id.

## Validation And Error Handling

Frontend validation prevents invalid TourLog forms:

- date/time must be valid
- difficulty must be selected
- total distance must be greater than zero
- total time must be greater than zero
- rating must be between 1 and 5

Backend DTO validation uses data annotations:

- `Required`
- `MaxLength`
- `Range`

If the authenticated user does not own the Tour or TourLog, the service returns no result and the API maps that to `404 NotFound`.

## Design Patterns Used

- DTO Pattern: separates API contracts from EF entities.
- Service Layer Pattern: keeps TourLog business rules in `TourLogService`.
- Repository Pattern: isolates EF Core persistence in `TourLogRepository`.
- Dependency Injection: injects repository and service dependencies.
- MVVM-style frontend state: components bind to service-owned state.

## Tests

TourLog behavior is covered in `TourLogServiceTests`.

The tests verify:

- creating logs for owned Tours
- rejecting creation for foreign Tours
- loading logs for owned Tours
- returning empty results for foreign Tours
- reading a single owned log
- updating owned logs
- rejecting updates for foreign logs
- deleting owned logs

Current note: the full test project may be blocked by unrelated existing Tour tests that assign `TransportType` values to string properties. The TourLog service implementation itself builds successfully.
