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
9. On success, `AuthService` generates a JWT through `ITokenService`.
10. The backend returns a `LoginResponseDto` with safe user data and a JWT token.
11. The frontend stores the authenticated user in `AuthService` state and stores the token in `localStorage`.
12. The frontend redirects the user to the tours view.

## Layer Responsibilities

### UI Layer

The Angular login component is responsible for form state, loading state and user feedback. It does not verify passwords and does not access persistence directly.

The Angular `AuthService` is responsible for API communication and authenticated user state. It stores the authenticated user in memory and stores the JWT token in `localStorage`.

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
- generate a JWT token
- return safe login response DTO with token

Invalid login attempts throw `InvalidCredentialsException`, which hides whether the username/email or password was wrong.

`TokenService` creates the JWT. The token contains only safe claims and is signed with the configured secret.

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
Token
```

`HashedPassword` is never sent to the frontend.

## Auth State

After a successful login, the Angular `AuthService` stores the authenticated user in an in-memory `BehaviorSubject` and stores the JWT in `localStorage`.

The service exposes:

- `currentUser$`
- `currentUser`
- `isAuthenticated`
- `accessToken`
- `logout()`

No password or password hash is stored in browser storage. `logout()` clears the token and the current user state.

## JWT Configuration

JWT settings are configured under the `Jwt` section in `appsettings.json` and `appsettings.Development.json`:

```text
Issuer
Audience
Secret
ExpirationMinutes
```

`Program.cs` configures JWT Bearer authentication with issuer, audience, lifetime and signing-key validation.

## Validation And Error Handling

Frontend validation prevents submitting an empty identifier or password.

Backend DTO validation uses data annotations:

- `Required`
- `MaxLength`

Invalid credentials return `401 Unauthorized`. The response does not reveal whether the identifier or password was incorrect.

Failed login attempts do not return a token.

## Design Patterns Used

- DTO Pattern: separates API contracts from EF entities.
- Service Layer Pattern: keeps login business logic in `AuthService`.
- Repository Pattern: isolates EF Core user lookup in `UserRepository`.
- Dependency Injection: injects repository, auth service and password hasher.
- Options Pattern: loads JWT settings through `JwtOptions`.

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
- successful login returns token
- token service creates JWT with issuer, audience, username claim and expiration
- token does not contain password/hash claims
