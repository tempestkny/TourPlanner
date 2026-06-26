using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface ITourRepository : IRepository<Tour>
{
    Task<IEnumerable<Tour>> ReadFromQuery(string userId,string? query);

}