using Microsoft.Extensions.Logging.Abstractions;
using TourPlanner.Bll.Auth;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class AuthServiceTests
{
    private FakeUserRepository userRepository;
    private Pbkdf2PasswordHasher passwordHasher;
    private FakeTokenService tokenService;
    private AuthService authService;

    [SetUp]
    public void Setup()
    {
        userRepository = new FakeUserRepository();
        passwordHasher = new Pbkdf2PasswordHasher();
        tokenService = new FakeTokenService();
        authService = new AuthService(
            userRepository,
            passwordHasher,
            tokenService,
            NullLogger<AuthService>.Instance);
    }

    [Test]
    public async Task Register_WithValidInput_ShouldCreateUser()
    {
        var result = await authService.Register(CreateRegisterDto());

        Assert.That(result.Id, Is.Not.Empty);
        Assert.That(userRepository.CreatedUsers, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Register_ShouldNormalizeEmail()
    {
        await authService.Register(CreateRegisterDto(email: "  TEST@Example.COM  "));

        Assert.That(userRepository.CreatedUsers.Single().Email, Is.EqualTo("test@example.com"));
    }

    [Test]
    public async Task Register_ShouldTrimUsername()
    {
        await authService.Register(CreateRegisterDto(username: "  Kenny  "));

        Assert.That(userRepository.CreatedUsers.Single().Username, Is.EqualTo("Kenny"));
    }

    [Test]
    public async Task Register_ShouldStorePasswordHashInsteadOfPlaintextPassword()
    {
        const string password = "SuperSecret123";

        await authService.Register(CreateRegisterDto(password: password));

        var storedHash = userRepository.CreatedUsers.Single().HashedPassword;
        Assert.That(storedHash, Is.Not.EqualTo(password));
        Assert.That(passwordHasher.VerifyPassword(password, storedHash!), Is.True);
    }

    [Test]
    public async Task Register_ShouldReturnSafeUserResponse()
    {
        var result = await authService.Register(CreateRegisterDto());

        Assert.That(result.Email, Is.EqualTo("user@example.com"));
        Assert.That(result.Username, Is.EqualTo("TourUser"));
        Assert.That(result.GetType().GetProperty("Password"), Is.Null);
        Assert.That(result.GetType().GetProperty("HashedPassword"), Is.Null);
    }

    [Test]
    public void Register_WithExistingEmail_ShouldThrowConflictException()
    {
        userRepository.ExistingEmails.Add("user@example.com");

        var exception = Assert.ThrowsAsync<RegistrationConflictException>(
            () => authService.Register(CreateRegisterDto()));

        Assert.That(exception!.Field, Is.EqualTo("email"));
    }

    [Test]
    public void Register_WithExistingUsername_ShouldThrowConflictException()
    {
        userRepository.ExistingUsernames.Add("TourUser");

        var exception = Assert.ThrowsAsync<RegistrationConflictException>(
            () => authService.Register(CreateRegisterDto()));

        Assert.That(exception!.Field, Is.EqualTo("username"));
    }

    [Test]
    public async Task Login_WithEmailAndValidPassword_ShouldReturnUser()
    {
        var user = CreateStoredUser();
        userRepository.CreatedUsers.Add(user);

        var result = await authService.Login(CreateLoginDto(identifier: "user@example.com"));

        Assert.That(result.Id, Is.EqualTo(user.Id));
        Assert.That(result.Email, Is.EqualTo(user.Email));
        Assert.That(result.Username, Is.EqualTo(user.Username));
        Assert.That(result.Token, Is.EqualTo(FakeTokenService.Token));
    }

    [Test]
    public async Task Login_WithUsernameAndValidPassword_ShouldReturnUser()
    {
        var user = CreateStoredUser();
        userRepository.CreatedUsers.Add(user);

        var result = await authService.Login(CreateLoginDto(identifier: "TourUser"));

        Assert.That(result.Id, Is.EqualTo(user.Id));
        Assert.That(result.Username, Is.EqualTo("TourUser"));
        Assert.That(result.Token, Is.EqualTo(FakeTokenService.Token));
    }

    [Test]
    public async Task Login_ShouldNormalizeEmailIdentifier()
    {
        var user = CreateStoredUser();
        userRepository.CreatedUsers.Add(user);

        var result = await authService.Login(CreateLoginDto(identifier: "  USER@Example.COM  "));

        Assert.That(result.Id, Is.EqualTo(user.Id));
    }

    [Test]
    public void Login_WithUnknownIdentifier_ShouldThrowInvalidCredentialsException()
    {
        Assert.ThrowsAsync<InvalidCredentialsException>(
            () => authService.Login(CreateLoginDto(identifier: "unknown@example.com")));
    }

    [Test]
    public void Login_WithWrongPassword_ShouldThrowInvalidCredentialsException()
    {
        userRepository.CreatedUsers.Add(CreateStoredUser());

        Assert.ThrowsAsync<InvalidCredentialsException>(
            () => authService.Login(CreateLoginDto(password: "WrongPassword123")));
    }

    [Test]
    public void Login_WithMissingPasswordHash_ShouldThrowInvalidCredentialsException()
    {
        userRepository.CreatedUsers.Add(new User
        {
            Email = "user@example.com",
            Username = "TourUser",
            HashedPassword = string.Empty
        });

        Assert.ThrowsAsync<InvalidCredentialsException>(
            () => authService.Login(CreateLoginDto()));
    }

    [Test]
    public async Task Login_ShouldReturnSafeUserResponse()
    {
        userRepository.CreatedUsers.Add(CreateStoredUser());

        var result = await authService.Login(CreateLoginDto());

        Assert.That(result.GetType().GetProperty("Password"), Is.Null);
        Assert.That(result.GetType().GetProperty("HashedPassword"), Is.Null);
    }

    [Test]
    public async Task Login_ShouldGenerateTokenForAuthenticatedUser()
    {
        var user = CreateStoredUser();
        userRepository.CreatedUsers.Add(user);

        var result = await authService.Login(CreateLoginDto());

        Assert.That(result.Token, Is.Not.Empty);
        Assert.That(tokenService.GeneratedForUsers.Single(), Is.EqualTo(user.Id));
    }

    private static RegisterUserDto CreateRegisterDto(
        string email = "user@example.com",
        string username = "TourUser",
        string password = "SuperSecret123")
    {
        return new RegisterUserDto
        {
            Email = email,
            Username = username,
            Password = password
        };
    }

    private LoginUserDto CreateLoginDto(
        string identifier = "user@example.com",
        string password = "SuperSecret123")
    {
        return new LoginUserDto
        {
            Identifier = identifier,
            Password = password
        };
    }

    private User CreateStoredUser(
        string email = "user@example.com",
        string username = "TourUser",
        string password = "SuperSecret123")
    {
        return new User
        {
            Email = email,
            Username = username,
            HashedPassword = passwordHasher.HashPassword(password)
        };
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> CreatedUsers { get; } = [];
        public HashSet<string> ExistingEmails { get; } = [];
        public HashSet<string> ExistingUsernames { get; } = [];

        public Task Create(User obj)
        {
            CreatedUsers.Add(obj);
            ExistingEmails.Add(obj.Email!);
            ExistingUsernames.Add(obj.Username!);
            return Task.CompletedTask;
        }

        public Task<User?> Read(string id)
        {
            return Task.FromResult(CreatedUsers.SingleOrDefault(user => user.Id == id));
        }

        public Task Update(string id, User objData)
        {
            return Task.CompletedTask;
        }

        public Task Delete(User obj)
        {
            CreatedUsers.Remove(obj);
            return Task.CompletedTask;
        }

        public Task<User?> GetUserByName(string username)
        {
            return Task.FromResult(CreatedUsers.SingleOrDefault(user => user.Username == username));
        }

        public Task<User?> GetUserByEmail(string email)
        {
            return Task.FromResult(CreatedUsers.SingleOrDefault(user => user.Email == email));
        }

        public Task<bool> ExistsByName(string username)
        {
            return Task.FromResult(ExistingUsernames.Contains(username));
        }

        public Task<bool> ExistsByEmail(string email)
        {
            return Task.FromResult(ExistingEmails.Contains(email));
        }

        public Task UpdatePassword(string id, string newPassword)
        {
            var user = CreatedUsers.SingleOrDefault(user => user.Id == id);
            if (user is not null)
            {
                user.HashedPassword = newPassword;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeTokenService : ITokenService
    {
        public const string Token = "test.jwt.token";

        public List<string> GeneratedForUsers { get; } = [];

        public string GenerateToken(User user)
        {
            GeneratedForUsers.Add(user.Id);
            return Token;
        }
    }
}
