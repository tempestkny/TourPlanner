using TourPlanner.Models;

namespace TourPlanner.Bll.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}
