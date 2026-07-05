using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class ImportExportService : IImportExportService
{
    private readonly ITourRepository tourRepository;
    private readonly ITourLogRepository tourLogRepository;
    private readonly ILogger<ImportExportService> logger;

    public ImportExportService(
        ITourRepository tourRepository,
        ITourLogRepository tourLogRepository,
        ILogger<ImportExportService> logger)
    {
        this.tourRepository = tourRepository;
        this.tourLogRepository = tourLogRepository;
        this.logger = logger;
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

        logger.LogInformation("Exported {TourCount} tours for user {UserId}", exportTours.Count, userId);

        return new ExportTourDataDto
        {
            ExportedAt = DateTime.UtcNow,
            Tours = exportTours
        };
    }

    public async Task<int> Import(string userId, ImportTourDataDto importData)
    {
        Validate(importData);
        logger.LogInformation("Starting import of {TourCount} tours for user {UserId}", importData.Tours.Count, userId);

        var importedTours = 0;

        foreach (var importTour in importData.Tours)
        {
            Validate(importTour);

            var tour = new Tour
            {
                Title = importTour.Title,
                Description = importTour.Description,
                From = importTour.From,
                To = importTour.To,
                TransportType = importTour.TransportType,
                Distance = importTour.Distance,
                Time = importTour.Time,
                UserId = userId
            };

            await tourRepository.Create(tour);

            foreach (var importTourLog in importTour.TourLogs)
            {
                Validate(importTourLog);

                var tourLog = new TourLog
                {
                    TourId = tour.Id,
                    TimeStamp = importTourLog.TimeStamp,
                    Comment = importTourLog.Comment,
                    Difficulty = importTourLog.Difficulty,
                    TotalDistance = importTourLog.TotalDistance,
                    TotalTime = importTourLog.TotalTime,
                    Rating = importTourLog.Rating,
                    Tour = null!
                };

                await tourLogRepository.Create(tourLog);
            }

            importedTours++;
        }

        logger.LogInformation("Imported {TourCount} tours for user {UserId}", importedTours, userId);

        return importedTours;
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

    private static void Validate<T>(T dto)
    {
        var validationContext = new ValidationContext(dto!);
        Validator.ValidateObject(dto!, validationContext, validateAllProperties: true);
    }
}
