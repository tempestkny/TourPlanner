using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll;

public interface IStatisticsService
{
    Task<StatisticsResponseDto> GetStatistics(string userId);
}
