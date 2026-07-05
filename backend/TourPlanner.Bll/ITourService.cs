using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll;
public interface ITourService
{
    public Task<TourResponseDto?> GetTour(string id);
    public Task<bool> RemoveTour(string id);
    public Task<string> CreateTour(string userId,CreateTourDto tour);
    public Task<bool> UpdateTour(string tourId, UpdateTourDto newTour);
    public Task<IEnumerable<TourResponseDto>?> GetTours(string userId, string? query = null);

    public Task<RouteInformationResponseDto?> GetRouteInformation(string id);
}