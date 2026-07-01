using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll.Auth;

public interface IAuthService
{
    Task<UserResponseDto> Register(RegisterUserDto registerUserDto);
    Task<LoginResponseDto> Login(LoginUserDto loginUserDto);
}
