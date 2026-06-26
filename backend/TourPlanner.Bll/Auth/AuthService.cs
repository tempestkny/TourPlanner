using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    public async Task<UserResponseDto> Register(RegisterUserDto registerUserDto)
    {
        var email = registerUserDto.Email.Trim().ToLowerInvariant();
        var username = registerUserDto.Username.Trim();

        if (await userRepository.ExistsByEmail(email))
        {
            logger.LogInformation("Registration rejected because email already exists: {Email}", email);
            throw new RegistrationConflictException("email");
        }

        if (await userRepository.ExistsByName(username))
        {
            logger.LogInformation("Registration rejected because username already exists: {Username}", username);
            throw new RegistrationConflictException("username");
        }

        var user = new User
        {
            Email = email,
            Username = username,
            HashedPassword = passwordHasher.HashPassword(registerUserDto.Password)
        };

        await userRepository.Create(user);

        logger.LogInformation("Registered user {UserId}", user.Id);

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username
        };
    }
}
