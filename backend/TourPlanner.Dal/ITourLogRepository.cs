using TourPlanner.Models;

namespace TourPlanner.Dal;

public interface ITourLogRepository : IRepository<TourLog>
{
    Task<IEnumerable<TourLog>> ReadByTourId(string tourId);
}
