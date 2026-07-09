using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll.Auth;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // creates route for controller, e.g. /api/auth
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService) // controller receives authService 
    {
        this.authService = authService;
    }

    [HttpPost("register")] // register endpoint
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterUserDto registerUserDto)
    {
        try
        {
            var registeredUser = await authService.Register(registerUserDto); // calls service
            return Created($"/api/users/{registeredUser.Id}", registeredUser); // returns status 201 with user info
        }
        catch (RegistrationConflictException exception) // if email or username already exists
        {
            return Conflict(new ProblemDetails
            {
                Title = "Registration conflict",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPost("login")] // login endpoint
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto loginUserDto)
    {
        try
        {
            var loggedInUser = await authService.Login(loginUserDto); // calls service to login user
            return Ok(loggedInUser); // returns status 200 with user info and token
        }
        catch (InvalidCredentialsException exception) // if email/username or password is invalid
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
