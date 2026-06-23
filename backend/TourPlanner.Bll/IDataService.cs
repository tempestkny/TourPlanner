using TourPlanner.Models;

namespace TourPlanner.Bll;

public interface IDataService<T>
{
    T GetValue(string id);
    void RemoveValue(string id);
    IEnumerable<T> GetValues(string? query);

}
