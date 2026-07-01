# Registration Flow

## Goal

The registration feature allows a new user to create an account with an email address, username and password. The implementation follows the layered architecture used in the project:

```text
Angular UI -> ASP.NET API Controller -> Business Logic Service -> Repository -> EF Core/PostgreSQL
```

Each layer only calls the layer directly below it.

## Flow

1. The Angular register component collects email, username, password and password confirmation.
2. The component validates that all fields are filled and that both passwords match.
3. The component calls `AuthService.register(...)` in the Angular frontend.
4. The Angular `AuthService` sends a `POST /api/auth/register` request to the backend.
5. The ASP.NET `AuthController` receives a `RegisterUserDto`.
6. The controller delegates registration to `IAuthService`.
7. `AuthService` checks whether the email or username already exists.
8. `AuthService` hashes the password before persistence.
9. `UserRepository` stores the new `User` entity through EF Core.
10. The backend returns a `UserResponseDto` without password or password hash.
11. The frontend redirects the user to the login page after successful registration.

## Layer Responsibilities

### UI Layer

The Angular register component is responsible for form state and user feedback. It does not contain persistence logic and does not generate the final user id. The backend creates the user id.

The Angular `AuthService` is responsible for communicating with the backend API. The API base URL is configured centrally so other frontend services can reuse it.

### API Layer

`AuthController` exposes the registration endpoint:

```text
POST /api/auth/register
```

The controller remains thin. It accepts DTOs, calls the business service and maps duplicate-user errors to `409 Conflict`.

### Business Logic Layer

`AuthService` contains the registration business rules:

- normalize email
- trim username
- reject duplicate email
- reject duplicate username
- hash password
- return safe response DTO

Password hashing is accessed through `IPasswordHasher`, which keeps the service testable and decoupled from the concrete hashing implementation.

### Data Access Layer

`UserRepository` is responsible for persistence only. It provides user lookup and duplicate-check methods used by the business layer. EF Core maps the `User` entity to PostgreSQL.

The `User` entity is configured with:

- primary key: `Id`
- required `Email`
- required `Username`
- required `HashedPassword`
- unique index on `Email`
- unique index on `Username`

## DTO Usage

The frontend and backend do not exchange EF entities directly.

`RegisterUserDto` is used for incoming registration data:

```text
Email
Username
Password
```

`UserResponseDto` is used for safe responses:

```text
Id
Email
Username
```

`HashedPassword` is never sent to the frontend.

## Validation

Frontend validation prevents submitting incomplete forms or mismatched passwords.

Backend DTO validation uses data annotations such as:

- `Required`
- `EmailAddress`
- `MinLength`
- `MaxLength`

The backend still performs duplicate checks in the business layer because uniqueness is a business rule and must not rely only on the UI.

## Security Decisions

Passwords are never logged and never stored as plaintext. Before saving a user, the business layer hashes the password through the password hasher abstraction.

The response DTO intentionally excludes both password and password hash fields.

## Design Patterns Used

- DTO Pattern: separates API contracts from EF entities.
- Service Layer Pattern: keeps registration business logic in `AuthService`.
- Repository Pattern: isolates EF Core persistence in `UserRepository`.
- Dependency Injection: injects repository, auth service and password hasher.

## Tests

Registration tests are split by responsibility:

- `AuthServiceTests` cover registration business logic.
- `PasswordHasherTests` cover password hashing and verification behavior.

The tests verify successful registration, duplicate email handling, duplicate username handling, password hashing, safe responses and password verification.
