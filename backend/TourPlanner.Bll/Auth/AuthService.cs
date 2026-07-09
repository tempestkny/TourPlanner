using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository; // loads & stores users
    private readonly IPasswordHasher passwordHasher; // hashes & verifies passwords
    private readonly ITokenService tokenService; // creates tokens 
    private readonly ILogger<AuthService> logger; // structured logging

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
        this.logger = logger;
    }

    public async Task<UserResponseDto> Register(RegisterUserDto registerUserDto)
    {
        var email = registerUserDto.Email.Trim().ToLowerInvariant();
        var username = registerUserDto.Username.Trim();

        if (await userRepository.ExistsByEmail(email)) // check if email already exists
        {
            logger.LogInformation("Registration rejected because email already exists: {Email}", email);
            throw new RegistrationConflictException("email");
        }

        if (await userRepository.ExistsByName(username)) // check if username already exists
        {
            logger.LogInformation("Registration rejected because username already exists: {Username}", username);
            throw new RegistrationConflictException("username");
        }

        var user = new User // create new user object
        {
            Email = email,
            Username = username,
            HashedPassword = passwordHasher.HashPassword(registerUserDto.Password) // hash the password
        };

        await userRepository.Create(user);

        logger.LogInformation("Registered user {UserId}", user.Id);

        return new UserResponseDto // dto to return user 
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username
        };
    }

    public async Task<LoginResponseDto> Login(LoginUserDto loginUserDto)
    {
        var identifier = loginUserDto.Identifier.Trim(); // email or username
        var normalizedEmail = identifier.ToLowerInvariant(); 

        var user = await userRepository.GetUserByEmail(normalizedEmail)
            ?? await userRepository.GetUserByName(identifier); // try to find user by email or username

        if (user is null || string.IsNullOrWhiteSpace(user.HashedPassword)) // login fails if user not found
        {
            logger.LogInformation("Login rejected for unknown identifier");
            throw new InvalidCredentialsException();
        }

        if (!passwordHasher.VerifyPassword(loginUserDto.Password, user.HashedPassword)) // password compared to stored hash
        {
            logger.LogInformation("Login rejected for user {UserId}", user.Id);
            throw new InvalidCredentialsException();
        }

        logger.LogInformation("User {UserId} logged in", user.Id);

        return new LoginResponseDto // retunr dto with user info and token
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Username = user.Username ?? string.Empty,
            Token = tokenService.GenerateToken(user)
        };
    }
}
