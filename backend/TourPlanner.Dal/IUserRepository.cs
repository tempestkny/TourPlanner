using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetUserByName(string username);
    public Task UpdatePassword(string id, string newPassword);
}