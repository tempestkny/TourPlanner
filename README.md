# TourPlanner

Full-stack tour planning web application built with Angular, ASP.NET Core Web API, Entity Framework Core and PostgreSQL.

## Prerequisites

Install the following tools before starting the application:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/)
- Docker Desktop
- npm

## Project Structure

```text
TourPlanner/
+-- backend/
|   +-- TourPlanner.Api/
|   +-- TourPlanner.Bll/
|   +-- TourPlanner.Dal/
|   +-- TourPlanner.Models/
|   +-- TourPlanner.Tests/
|   +-- docker-compose.yml
+-- frontend/
    +-- TourPlanner.Frontend/
```

## Start The Application

Run the following commands from the repository root.

### 1. Start PostgreSQL

Make sure Docker Desktop is running, then start the database:

```powershell
cd backend
docker compose up -d
cd ..
```

The local PostgreSQL database uses:

```text
Host: localhost
Port: 5432
Database: tourplannerdb
Username: admin
Password: SWENSS26
```

### 2. Start The Backend

```powershell
dotnet run --project backend\TourPlanner.Api\TourPlanner.Api.csproj --launch-profile http
```

The API runs on:

```text
http://localhost:5020
```

Useful backend URLs:

```text
Swagger:      http://localhost:5020/swagger
Health Check: http://localhost:5020/health
```

### 3. Start The Frontend

Open a second terminal:

```powershell
cd frontend\TourPlanner.Frontend
npm install
npm start
```

The Angular application runs on:

```text
http://localhost:4200
```

The frontend API base URL is configured in:

```text
frontend/TourPlanner.Frontend/src/app/api.config.ts
```

Default value:

```ts
export const API_BASE_URL = 'http://localhost:5020/api';
```

## First Manual Test

1. Open `http://localhost:4200`.
2. Register a new user.
3. Log in with the created user.
4. Create a tour.
5. Add a tour log.
6. Test import/export from the tour list.
7. Open the statistics dashboard.

## Authentication

The backend uses JWT authentication.

After login, the frontend stores the JWT and sends it with protected API requests. Swagger also supports JWT authorization through the `Authorize` button.

## OpenRouteService

Route calculation uses OpenRouteService through the backend.

For local development, the API key can be configured in:

```text
backend/TourPlanner.Api/appsettings.json
backend/TourPlanner.Api/Properties/launchSettings.json
```

or through the environment variable:

```text
ORS_API_KEY
```

## Common Problems

### Docker Error: Cannot Connect To Docker Engine

Start Docker Desktop first, then run:

```powershell
cd backend
docker compose up -d
```

### Frontend Cannot Reach Backend

Check that the backend is running on `http://localhost:5020`.

Also check:

```text
frontend/TourPlanner.Frontend/src/app/api.config.ts
```

### Unauthorized Requests

Log out, clear browser local storage, then register or log in again.

In Chrome DevTools:

```text
Application -> Local Storage -> http://localhost:4200 -> Clear
```

### Swagger Is Not Opening

Use the HTTP URL:

```text
http://localhost:5020/swagger
```

If the backend was started with the HTTPS launch profile, Swagger may also be available on:

```text
https://localhost:7161/swagger
```

## Build And Test

Backend build:

```powershell
dotnet build backend\TourPlanner.Api\TourPlanner.Api.csproj
```

Frontend build:

```powershell
cd frontend\TourPlanner.Frontend
npm run build
```

Backend tests:

```powershell
dotnet test backend\TourPlanner.Tests\TourPlanner.Tests.csproj
```
