using TourPlanner.Models;

namespace TourPlanner.Dal;

public class UserRepository : Repository, IRepository<User>
{
    public UserRepository(TourPlannerDbContext context) : base(context)
    {
    }
    public void Create(User obj)
    {
        throw new NotImplementedException();
    }

    public void Delete(User obj)
    {
        throw new NotImplementedException();
    }

    public Task<User?> Read(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> ReadAll(string? query = null)
    {
        throw new NotImplementedException();
    }

    public void Update(string id, User objData)
    {
        throw new NotImplementedException();
    }
}