# Login Flow

## Goal

The login feature allows an existing user to authenticate with either username or email plus password. The implementation follows the same layered architecture as the registration feature:

```text
Angular UI -> ASP.NET API Controller -> Business Logic Service -> Repository -> EF Core/PostgreSQL
```

Each layer only calls the layer directly below it.

## Flow

1. The Angular login component collects username/email and password.
2. The component validates that both fields are filled.
3. The component calls `AuthService.login(...)` in the Angular frontend.
4. The Angular `AuthService` sends a `POST /api/auth/login` request to the backend.
5. The ASP.NET `AuthController` receives a `LoginUserDto`.
6. The controller delegates login logic to `IAuthService`.
7. `AuthService` looks up the user by normalized email or username.
8. `AuthService` verifies the password through `IPasswordHasher`.
9. On success, the backend returns a `LoginResponseDto` without password or password hash.
10. The frontend stores the authenticated user in `AuthService` state.
11. The frontend redirects the user to the tours view.

## Layer Responsibilities

### UI Layer

The Angular login component is responsible for form state, loading state and user feedback. It does not verify passwords and does not access persistence directly.

The Angular `AuthService` is responsible for API communication and authenticated user state. The current implementation stores the authenticated user in memory only.

### API Layer

`AuthController` exposes the login endpoint:

```text
POST /api/auth/login
```

The controller remains thin. It accepts `LoginUserDto`, calls `IAuthService.Login(...)` and maps invalid credentials to `401 Unauthorized`.

### Business Logic Layer

`AuthService` contains the login business rules:

- trim the submitted identifier
- normalize the identifier for email lookup
- find user by email or username
- reject unknown users
- reject missing password hashes
- verify password hash
- return safe login response DTO

Invalid login attempts throw `InvalidCredentialsException`, which hides whether the username/email or password was wrong.

### Data Access Layer

`UserRepository` provides user lookup methods used by the business layer:

- `GetUserByEmail`
- `GetUserByName`

The repository does not verify passwords and does not contain authentication business logic.

## DTO Usage

The frontend and backend do not exchange EF entities directly.

`LoginUserDto` is used for incoming login data:

```text
Identifier
Password
```

`LoginResponseDto` is used for safe responses:

```text
Id
Email
Username
```

`HashedPassword` is never sent to the frontend.

## Auth State

After a successful login, the Angular `AuthService` stores the authenticated user in an in-memory `BehaviorSubject`.

The service exposes:

- `currentUser$`
- `currentUser`
- `isAuthenticated`
- `logout()`

No password, password hash or token is stored in browser storage.

## Validation And Error Handling

Frontend validation prevents submitting an empty identifier or password.

Backend DTO validation uses data annotations:

- `Required`
- `MaxLength`

Invalid credentials return `401 Unauthorized`. The response does not reveal whether the identifier or password was incorrect.

## Design Patterns Used

- DTO Pattern: separates API contracts from EF entities.
- Service Layer Pattern: keeps login business logic in `AuthService`.
- Repository Pattern: isolates EF Core user lookup in `UserRepository`.
- Dependency Injection: injects repository, auth service and password hasher.

## Tests

Login behavior is covered in `AuthServiceTests`.

The tests verify:

- login with email
- login with username
- email identifier normalization
- unknown identifier rejection
- wrong password rejection
- missing password hash rejection
- safe response DTO without password/hash
