using TourPlanner.Models;
namespace TourPlanner.Bll;
public interface ITourService : IDataService<TourDto>
{
    public Task<string> CreateTour(string userId,TourDto tour);
    public Task<bool> UpdateTour(string tourId, TourDto newTour);
    public Task<IEnumerable<TourDto>?> GetTours(string userId, string? query = null);
    protected TourDto? convertToDto(Tour tour);
}