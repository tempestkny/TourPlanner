using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> getUserByName(string username);
}