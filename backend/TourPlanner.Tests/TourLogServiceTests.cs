using Microsoft.Extensions.Logging.Abstractions;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class TourLogServiceTests
{
    private const string UserId = "user-1";
    private const string OtherUserId = "user-2";

    private FakeTourRepository tourRepository;
    private FakeTourLogRepository tourLogRepository;
    private TourLogService tourLogService;

    [SetUp]
    public void SetUp()
    {
        tourRepository = new FakeTourRepository();
        tourLogRepository = new FakeTourLogRepository();
        tourLogService = new TourLogService(tourLogRepository, tourRepository, NullLogger<TourLogService>.Instance);
    }

    [Test]
    public async Task Create_WithOwnedTour_ShouldCreateTourLog()
    {
        var tour = CreateTour(UserId);
        tourRepository.Tours.Add(tour);

        var result = await tourLogService.Create(UserId, CreateTourLogDto(tour.Id));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TourId, Is.EqualTo(tour.Id));
        Assert.That(result.Difficulty, Is.EqualTo(Difficulty.Medium));
        Assert.That(tourLogRepository.TourLogs, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Create_WithForeignTour_ShouldReturnNullAndNotCreateTourLog()
    {
        var tour = CreateTour(OtherUserId);
        tourRepository.Tours.Add(tour);

        var result = await tourLogService.Create(UserId, CreateTourLogDto(tour.Id));

        Assert.That(result, Is.Null);
        Assert.That(tourLogRepository.TourLogs, Is.Empty);
    }

    [Test]
    public async Task GetByTourId_WithOwnedTour_ShouldReturnMappedTourLogs()
    {
        var tour = CreateTour(UserId);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(CreateTourLog(tour.Id, rating: 5));
        tourLogRepository.TourLogs.Add(CreateTourLog(tour.Id, rating: 3));

        var result = await tourLogService.GetByTourId(UserId, tour.Id);

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(log => log.Rating), Is.EquivalentTo(new[] { 5, 3 }));
    }

    [Test]
    public async Task GetByTourId_WithForeignTour_ShouldReturnEmptyList()
    {
        var tour = CreateTour(OtherUserId);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(CreateTourLog(tour.Id));

        var result = await tourLogService.GetByTourId(UserId, tour.Id);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Get_WithOwnedTourLog_ShouldReturnTourLog()
    {
        var tour = CreateTour(UserId);
        var tourLog = CreateTourLog(tour.Id);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(tourLog);

        var result = await tourLogService.Get(UserId, tourLog.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(tourLog.Id));
        Assert.That(result.TourId, Is.EqualTo(tour.Id));
    }

    [Test]
    public async Task Update_WithOwnedTourLog_ShouldUpdateTourLog()
    {
        var tour = CreateTour(UserId);
        var tourLog = CreateTourLog(tour.Id);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(tourLog);

        var result = await tourLogService.Update(UserId, tourLog.Id, new UpdateTourLogDto
        {
            TimeStamp = new DateTime(2026, 7, 4, 12, 0, 0, DateTimeKind.Utc),
            Comment = "Updated comment",
            Difficulty = Difficulty.Hard,
            TotalDistance = 12000,
            TotalTime = 95,
            Rating = 4
        });

        Assert.That(result, Is.True);
        Assert.That(tourLog.Comment, Is.EqualTo("Updated comment"));
        Assert.That(tourLog.Difficulty, Is.EqualTo(Difficulty.Hard));
        Assert.That(tourLog.Rating, Is.EqualTo(4));
    }

    [Test]
    public async Task Update_WithForeignTourLog_ShouldReturnFalse()
    {
        var tour = CreateTour(OtherUserId);
        var tourLog = CreateTourLog(tour.Id);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(tourLog);

        var result = await tourLogService.Update(UserId, tourLog.Id, new UpdateTourLogDto
        {
            TimeStamp = DateTime.UtcNow,
            Comment = "Should not update",
            Difficulty = Difficulty.Hard,
            TotalDistance = 1,
            TotalTime = 1,
            Rating = 1
        });

        Assert.That(result, Is.False);
        Assert.That(tourLog.Comment, Is.Not.EqualTo("Should not update"));
    }

    [Test]
    public async Task Remove_WithOwnedTourLog_ShouldDeleteTourLog()
    {
        var tour = CreateTour(UserId);
        var tourLog = CreateTourLog(tour.Id);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(tourLog);

        var result = await tourLogService.Remove(UserId, tourLog.Id);

        Assert.That(result, Is.True);
        Assert.That(tourLogRepository.TourLogs, Is.Empty);
    }

    private static CreateTourLogDto CreateTourLogDto(string tourId)
    {
        return new CreateTourLogDto
        {
            TourId = tourId,
            TimeStamp = new DateTime(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc),
            Comment = "Nice tour",
            Difficulty = Difficulty.Medium,
            TotalDistance = 10000,
            TotalTime = 80,
            Rating = 5
        };
    }

    private static Tour CreateTour(string userId)
    {
        return new Tour
        {
            UserId = userId,
            Title = "Test tour",
            From = "Vienna",
            To = "Graz",
            TransportType = TransportType.Bike
        };
    }

    private static TourLog CreateTourLog(string tourId, int rating = 5)
    {
        return new TourLog
        {
            TourId = tourId,
            TimeStamp = new DateTime(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc),
            Comment = "Existing log",
            Difficulty = Difficulty.Easy,
            TotalDistance = 5000,
            TotalTime = 45,
            Rating = rating,
            Tour = null!
        };
    }

    private sealed class FakeTourRepository : ITourRepository
    {
        public List<Tour> Tours { get; } = [];

        public Task Create(Tour obj)
        {
            Tours.Add(obj);
            return Task.CompletedTask;
        }

        public Task<Tour?> Read(string id)
        {
            return Task.FromResult(Tours.SingleOrDefault(tour => tour.Id == id));
        }

        public Task Update(string id, Tour objData)
        {
            return Task.CompletedTask;
        }

        public Task Delete(Tour obj)
        {
            Tours.Remove(obj);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Tour>> ReadFromQuery(string userId, string? query)
        {
            return Task.FromResult(Tours.Where(tour => tour.UserId == userId).AsEnumerable());
        }
    }

    private sealed class FakeTourLogRepository : ITourLogRepository
    {
        public List<TourLog> TourLogs { get; } = [];

        public Task Create(TourLog obj)
        {
            TourLogs.Add(obj);
            return Task.CompletedTask;
        }

        public Task<TourLog?> Read(string id)
        {
            return Task.FromResult(TourLogs.SingleOrDefault(tourLog => tourLog.Id == id));
        }

        public Task<IEnumerable<TourLog>> ReadByTourId(string tourId)
        {
            return Task.FromResult(TourLogs.Where(tourLog => tourLog.TourId == tourId).AsEnumerable());
        }

        public Task Update(string id, TourLog objData)
        {
            var tourLog = TourLogs.SingleOrDefault(tourLog => tourLog.Id == id);
            if (tourLog is not null)
            {
                tourLog.TimeStamp = objData.TimeStamp;
                tourLog.Comment = objData.Comment;
                tourLog.Difficulty = objData.Difficulty;
                tourLog.TotalDistance = objData.TotalDistance;
                tourLog.TotalTime = objData.TotalTime;
                tourLog.Rating = objData.Rating;
            }

            return Task.CompletedTask;
        }

        public Task Delete(TourLog obj)
        {
            TourLogs.Remove(obj);
            return Task.CompletedTask;
        }
    }
}
