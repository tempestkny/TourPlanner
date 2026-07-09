using Microsoft.Extensions.Logging;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class TourLogService : ITourLogService
{
    private readonly ITourLogRepository tourLogRepository;
    private readonly ITourRepository tourRepository;
    private readonly ILogger<TourLogService> logger;

    public TourLogService(
        ITourLogRepository tourLogRepository,
        ITourRepository tourRepository,
        ILogger<TourLogService> logger)
    {
        this.tourLogRepository = tourLogRepository;
        this.tourRepository = tourRepository;
        this.logger = logger;
    }

    public async Task<TourLogResponseDto?> Get(string userId, string tourLogId)
    {
        var tourLog = await tourLogRepository.Read(tourLogId); // load DB
        if (tourLog is null || !await UserOwnsTour(userId, tourLog.TourId)) return null; // check ownership and existence

        return ToDto(tourLog);
    }

    public async Task<IEnumerable<TourLogResponseDto>> GetByTourId(string userId, string tourId)
    {
        if (!await UserOwnsTour(userId, tourId)) return [];

        var tourLogs = await tourLogRepository.ReadByTourId(tourId); // load logs
        return tourLogs.Select(ToDto).ToList(); // map entity to dto
    }

    public async Task<TourLogResponseDto?> Create(string userId, CreateTourLogDto tourLogDto)
    {
        if (!await UserOwnsTour(userId, tourLogDto.TourId)) // check ownership
        {
            logger.LogWarning("User {UserId} tried to create a tour log for tour {TourId} without ownership", userId, tourLogDto.TourId);
            return null;
        }

        var tourLog = new TourLog // map dto to entity
        {
            TourId = tourLogDto.TourId, // tourId is enough as foreign key, no need to load the tour entity
            TimeStamp = tourLogDto.TimeStamp,
            Comment = tourLogDto.Comment,
            Difficulty = tourLogDto.Difficulty,
            TotalDistance = tourLogDto.TotalDistance,
            TotalTime = tourLogDto.TotalTime,
            Rating = tourLogDto.Rating,
            Tour = null!
        };

        await tourLogRepository.Create(tourLog); // saving in db
        logger.LogInformation("Created tour log {TourLogId} for tour {TourId}", tourLog.Id, tourLog.TourId);

        return ToDto(tourLog);
    }

    public async Task<bool> Update(string userId, string tourLogId, UpdateTourLogDto tourLogDto)
    {
        var existingTourLog = await tourLogRepository.Read(tourLogId); // load existing log
        if (existingTourLog is null || !await UserOwnsTour(userId, existingTourLog.TourId)) // check ownership and existence
        {
            logger.LogWarning("User {UserId} tried to update tour log {TourLogId} without ownership", userId, tourLogId);
            return false;
        }

        var updatedTourLog = new TourLog // entity with updated values, keeping the same ID and TourId
        {
            TimeStamp = tourLogDto.TimeStamp,
            Comment = tourLogDto.Comment,
            Difficulty = tourLogDto.Difficulty,
            TotalDistance = tourLogDto.TotalDistance,
            TotalTime = tourLogDto.TotalTime,
            Rating = tourLogDto.Rating,
            Tour = null!
        };

        await tourLogRepository.Update(tourLogId, updatedTourLog); // update in db
        logger.LogInformation("Updated tour log {TourLogId}", tourLogId);
        return true;
    }

    public async Task<bool> Remove(string userId, string tourLogId)
    {
        var tourLog = await tourLogRepository.Read(tourLogId); // load existing log
        if (tourLog is null || !await UserOwnsTour(userId, tourLog.TourId)) // check ownership and existence
        {
            logger.LogWarning("User {UserId} tried to delete tour log {TourLogId} without ownership", userId, tourLogId);
            return false;
        }

        await tourLogRepository.Delete(tourLog); // delete from db
        logger.LogInformation("Deleted tour log {TourLogId}", tourLogId);
        return true;
    }

    private async Task<bool> UserOwnsTour(string userId, string? tourId)
    {
        if (string.IsNullOrWhiteSpace(tourId)) return false; 

        var tour = await tourRepository.Read(tourId); // load tour from db
        return tour?.UserId == userId; // check if the user owns the tour
    }

    private static TourLogResponseDto ToDto(TourLog tourLog) // map entity to dto
    {
        return new TourLogResponseDto
        {
            Id = tourLog.Id,
            TourId = tourLog.TourId ?? string.Empty,
            TimeStamp = tourLog.TimeStamp,
            Comment = tourLog.Comment,
            Difficulty = tourLog.Difficulty,
            TotalDistance = tourLog.TotalDistance,
            TotalTime = tourLog.TotalTime,
            Rating = tourLog.Rating
        };
    }
}
