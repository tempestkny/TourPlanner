using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetUserByName(string username);
    public Task<User?> GetUserByEmail(string email);
    public Task<bool> ExistsByName(string username);
    public Task<bool> ExistsByEmail(string email);
    public Task UpdatePassword(string id, string newPassword);
}
