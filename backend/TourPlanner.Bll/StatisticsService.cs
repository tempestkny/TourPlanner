using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class StatisticsService : IStatisticsService
{
    private readonly ITourRepository tourRepository;
    private readonly ITourLogRepository tourLogRepository;
    private readonly ILogger<StatisticsService> logger;

    public StatisticsService(
        ITourRepository tourRepository,
        ITourLogRepository tourLogRepository,
        ILogger<StatisticsService> logger)
    {
        this.tourRepository = tourRepository;
        this.tourLogRepository = tourLogRepository;
        this.logger = logger;
    }

    public async Task<StatisticsResponseDto> GetStatistics(string userId)
    {
        var tours = (await tourRepository.ReadFromQuery(userId, null)).ToList();
        var tourLogs = new List<TourLog>();

        foreach (var tour in tours)
        {
            var logs = await tourLogRepository.ReadByTourId(tour.Id);
            tourLogs.AddRange(logs);
        }

        logger.LogInformation("Calculated statistics for user {UserId}", userId);

        return new StatisticsResponseDto
        {
            TotalTours = tours.Count,
            TotalTourLogs = tourLogs.Count,
            TotalDistance = Math.Round(tours.Sum(tour => tour.Distance), 2),
            TotalTime = Math.Round(tours.Sum(tour => tour.Time), 2),
            AverageRating = tourLogs.Count == 0
                ? 0
                : Math.Round(tourLogs.Average(tourLog => tourLog.Rating), 2)
        };
    }
}
