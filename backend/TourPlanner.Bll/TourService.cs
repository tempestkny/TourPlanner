using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;

    public TourService(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public Tour GetValue(string id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Tour> GetValues(string? query)
    {
        throw new NotImplementedException();
    }

    public void RemoveValue(string id)
    {
        throw new NotImplementedException();
    }
}