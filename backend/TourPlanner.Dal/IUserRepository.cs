using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetUserByName(string username);
    public void UpdatePassword(string id, string newPassword);
}