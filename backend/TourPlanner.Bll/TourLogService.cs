using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class TourLogService : ITourLogService
{
    private readonly ITourLogRepository tourLogRepository;
    private readonly ITourRepository tourRepository;

    public TourLogService(ITourLogRepository tourLogRepository, ITourRepository tourRepository)
    {
        this.tourLogRepository = tourLogRepository;
        this.tourRepository = tourRepository;
    }

    public async Task<TourLogResponseDto?> Get(string userId, string tourLogId)
    {
        var tourLog = await tourLogRepository.Read(tourLogId);
        if (tourLog is null || !await UserOwnsTour(userId, tourLog.TourId)) return null;

        return ToDto(tourLog);
    }

    public async Task<IEnumerable<TourLogResponseDto>> GetByTourId(string userId, string tourId)
    {
        if (!await UserOwnsTour(userId, tourId)) return [];

        var tourLogs = await tourLogRepository.ReadByTourId(tourId);
        return tourLogs.Select(ToDto).ToList();
    }

    public async Task<TourLogResponseDto?> Create(string userId, CreateTourLogDto tourLogDto)
    {
        if (!await UserOwnsTour(userId, tourLogDto.TourId)) return null;

        var tourLog = new TourLog
        {
            TourId = tourLogDto.TourId,
            TimeStamp = tourLogDto.TimeStamp,
            Comment = tourLogDto.Comment,
            Difficulty = tourLogDto.Difficulty,
            TotalDistance = tourLogDto.TotalDistance,
            TotalTime = tourLogDto.TotalTime,
            Rating = tourLogDto.Rating,
            Tour = null!
        };

        await tourLogRepository.Create(tourLog);

        return ToDto(tourLog);
    }

    public async Task<bool> Update(string userId, string tourLogId, UpdateTourLogDto tourLogDto)
    {
        var existingTourLog = await tourLogRepository.Read(tourLogId);
        if (existingTourLog is null || !await UserOwnsTour(userId, existingTourLog.TourId)) return false;

        var updatedTourLog = new TourLog
        {
            TimeStamp = tourLogDto.TimeStamp,
            Comment = tourLogDto.Comment,
            Difficulty = tourLogDto.Difficulty,
            TotalDistance = tourLogDto.TotalDistance,
            TotalTime = tourLogDto.TotalTime,
            Rating = tourLogDto.Rating,
            Tour = null!
        };

        await tourLogRepository.Update(tourLogId, updatedTourLog);
        return true;
    }

    public async Task<bool> Remove(string userId, string tourLogId)
    {
        var tourLog = await tourLogRepository.Read(tourLogId);
        if (tourLog is null || !await UserOwnsTour(userId, tourLog.TourId)) return false;

        await tourLogRepository.Delete(tourLog);
        return true;
    }

    private async Task<bool> UserOwnsTour(string userId, string? tourId)
    {
        if (string.IsNullOrWhiteSpace(tourId)) return false;

        var tour = await tourRepository.Read(tourId);
        return tour?.UserId == userId;
    }

    private static TourLogResponseDto ToDto(TourLog tourLog)
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
