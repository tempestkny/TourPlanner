using TourPlanner.Models;

namespace TourPlanner.Bll;

public interface IDataService<T>
{
    Task<T?> Get(string id);
    Task<bool> Remove(string id);
}
