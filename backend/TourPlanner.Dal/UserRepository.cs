using TourPlanner.Models;

namespace TourPlanner.Dal;

public class UserRepository : Repository, IUserRepository
{
    public UserRepository(TourPlannerDbContext context) : base(context)
    {
    }

    public async void Create(User user)
    {
        _context.users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(User obj)
    {
        _context.users.Remove(obj);
        await _context.SaveChangesAsync();
    }

    public void Update(string id, User objData)
    {
        throw new NotImplementedException();
    }
}