using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class ImportExportService : IImportExportService
{
    private readonly ITourRepository tourRepository;
    private readonly ITourLogRepository tourLogRepository;

    public ImportExportService(ITourRepository tourRepository, ITourLogRepository tourLogRepository)
    {
        this.tourRepository = tourRepository;
        this.tourLogRepository = tourLogRepository;
    }

    public async Task<ExportTourDataDto> Export(string userId)
    {
        var tours = await tourRepository.ReadFromQuery(userId, null);
        var exportTours = new List<ExportTourDto>();

        foreach (var tour in tours)
        {
            var tourLogs = await tourLogRepository.ReadByTourId(tour.Id);
            exportTours.Add(ToExportTourDto(tour, tourLogs));
        }

        return new ExportTourDataDto
        {
            ExportedAt = DateTime.UtcNow,
            Tours = exportTours
        };
    }

    private static ExportTourDto ToExportTourDto(Tour tour, IEnumerable<TourLog> tourLogs)
    {
        return new ExportTourDto
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            From = tour.From,
            To = tour.To,
            TransportType = tour.TransportType,
            Distance = tour.Distance,
            Time = tour.Time,
            TourLogs = tourLogs.Select(ToExportTourLogDto).ToList()
        };
    }

    private static ExportTourLogDto ToExportTourLogDto(TourLog tourLog)
    {
        return new ExportTourLogDto
        {
            Id = tourLog.Id,
            TimeStamp = tourLog.TimeStamp,
            Comment = tourLog.Comment,
            Difficulty = tourLog.Difficulty,
            TotalDistance = tourLog.TotalDistance,
            TotalTime = tourLog.TotalTime,
            Rating = tourLog.Rating
        };
    }
}
