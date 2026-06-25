using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Dal;

public class UserRepository : Repository, IUserRepository
{
    public UserRepository(TourPlannerDbContext context) : base(context)
    {
    }

    public async Task Create(User user)
    {
        _context.users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(User obj)
    {
        _context.users.Remove(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByName(string username) => await _context.users.Where(u => u.Username == username).FirstAsync();
    public async Task<User?> Read(string id) => await _context.users.FindAsync(id);

    public async Task Update(string id, User objData)
    {
        var user = await _context.users.FindAsync(id);
        if(user is null) return;

        user.Email = objData.Email;
        user.Username = objData.Username;
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePassword(string id, string newPassword)
    {
        var user = await _context.users.FindAsync(id);
        if(user is null) return;

        user.HashedPassword = newPassword;
        
        await _context.SaveChangesAsync();
    }
}