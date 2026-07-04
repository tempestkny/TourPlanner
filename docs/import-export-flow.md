# Import/Export Flow

## Goal

Import/export allows an authenticated user to back up and restore TourPlanner data. The export contains the user's Tours and their related TourLogs. Import creates new Tours and TourLogs for the authenticated user.

The implementation follows the project architecture:

```text
Angular UI -> ASP.NET API Controller -> Business Logic Service -> Repository -> EF Core/PostgreSQL
```

Each layer only calls the layer directly below it.

## Flow

### Export

1. The user clicks `Export` in the Angular Tour overview.
2. The frontend calls `ImportExportService.exportTours()`.
3. The HTTP interceptor adds the JWT to the request.
4. `ImportExportController` reads the authenticated user id from the JWT.
5. The controller delegates to `IImportExportService.Export(...)`.
6. `ImportExportService` loads only Tours owned by the authenticated user.
7. For each Tour, the service loads the related TourLogs.
8. The service maps data to export DTOs.
9. The API returns JSON data.
10. The Angular UI downloads the response as a `.json` file.

### Import

1. The user clicks `Import` in the Angular Tour overview and selects a JSON file.
2. The frontend reads and parses the JSON file.
3. The frontend calls `ImportExportService.importTours(...)`.
4. The HTTP interceptor adds the JWT to the request.
5. `ImportExportController` reads the authenticated user id from the JWT.
6. The controller delegates to `IImportExportService.Import(...)`.
7. `ImportExportService` validates the import DTOs.
8. The service creates new Tours for the authenticated user.
9. The service creates related TourLogs for each imported Tour.
10. The Angular UI reloads the Tour list after successful import.

## API Endpoints

All import/export endpoints are protected with JWT authentication.

```text
GET  /api/import-export/export
POST /api/import-export/import
```

## Layer Responsibilities

### UI Layer

The Angular Tour overview exposes the import and export actions.

The Angular `ImportExportService` performs HTTP communication with the backend:

- `exportTours()`
- `importTours(...)`

The component handles file selection, JSON parsing, download creation, loading state and user feedback.

### API Layer

`ImportExportController` exposes the import/export endpoints and remains thin. It reads the user id from the JWT:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

The controller delegates all import/export logic to `IImportExportService`.

### Business Logic Layer

`ImportExportService` contains import/export business rules:

- export only authenticated user's Tours
- include related TourLogs in exports
- validate import DTOs
- create imported Tours for the authenticated user
- create imported TourLogs for the newly created Tours
- ignore any ownership information from import files
- map between entities and DTOs

### Data Access Layer

Repositories are responsible for persistence only:

- `ITourRepository` reads and creates Tours.
- `ITourLogRepository` reads and creates TourLogs.

Repositories do not make ownership decisions.

## DTO Usage

The frontend and backend do not exchange EF entities directly.

Export DTOs:

```text
ExportTourDataDto
ExportTourDto
ExportTourLogDto
```

Import DTOs:

```text
ImportTourDataDto
ImportTourDto
ImportTourLogDto
```

The export payload contains Tour IDs and TourLog IDs for traceability. Import payloads do not contain `UserId`. Imported data is assigned to the authenticated user from the JWT.

## Authentication And Ownership

The frontend does not send a user id for import/export ownership decisions.

The backend uses the validated JWT as the source of truth. The user id is read from `ClaimTypes.NameIdentifier`.

Import files are not trusted for ownership. Even if a JSON file contains additional fields, the import DTOs do not use them and the service assigns `Tour.UserId` from the authenticated user id.

## Validation And Error Handling

Import DTOs use data annotations:

- `Required`
- `MaxLength`
- `Range`

Invalid import payloads throw `ValidationException` in the business layer. The API maps validation failures to `400 BadRequest` with a `ProblemDetails` response.

Unauthorized requests return `401 Unauthorized`.

## Design Patterns Used

- DTO Pattern: separates import/export contracts from EF entities.
- Service Layer Pattern: keeps import/export business rules in `ImportExportService`.
- Repository Pattern: isolates EF Core persistence in repositories.
- Dependency Injection: injects repositories and service dependencies.
- MVVM-style frontend state: the UI binds to component state and delegates API calls to services.

## Tests

Import/export behavior is covered in `ImportExportServiceTests`.

The tests verify:

- exporting owned Tours with related TourLogs
- excluding foreign user data from exports
- importing valid Tours and TourLogs
- assigning imported Tours to the authenticated user
- rejecting invalid import payloads

## Manual Verification

1. Start PostgreSQL and the backend.
2. Start the Angular frontend.
3. Register or log in.
4. Create at least one Tour.
5. Add at least one TourLog to the Tour.
6. Click `Export` in the Tour overview.
7. Verify that a JSON file is downloaded.
8. Click `Import` and select the exported JSON file.
9. Verify that imported Tours appear in the Tour list.

Swagger can also be used:

1. Login through `/api/auth/login`.
2. Copy the JWT token.
3. Click `Authorize` in Swagger and paste the token.
4. Call `GET /api/import-export/export`.
5. Call `POST /api/import-export/import` with a valid import payload.
