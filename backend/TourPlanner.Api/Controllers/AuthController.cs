using Microsoft.AspNetCore.Mvc;
using TourPlanner.Bll.Auth;
using TourPlanner.Bll.Dtos;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterUserDto registerUserDto)
    {
        try
        {
            var registeredUser = await authService.Register(registerUserDto);
            return Created($"/api/users/{registeredUser.Id}", registeredUser);
        }
        catch (RegistrationConflictException exception)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Registration conflict",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
