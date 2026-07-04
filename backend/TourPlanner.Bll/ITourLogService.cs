using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll;

public interface ITourLogService
{
    Task<TourLogResponseDto?> Get(string userId, string tourLogId);
    Task<IEnumerable<TourLogResponseDto>> GetByTourId(string userId, string tourId);
    Task<TourLogResponseDto?> Create(string userId, CreateTourLogDto tourLogDto);
    Task<bool> Update(string userId, string tourLogId, UpdateTourLogDto tourLogDto);
    Task<bool> Remove(string userId, string tourLogId);
}
