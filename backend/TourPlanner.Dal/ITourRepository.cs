using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface ITourRepository : IRepository<Tour>
{
    Task<Tour> ReadFromQuery(string userId,string? query);
    
}