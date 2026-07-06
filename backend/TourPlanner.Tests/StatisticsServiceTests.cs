using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using TourPlanner.Bll;
using TourPlanner.Dal;
using TourPlanner.Models;
using TourPlanner.Models.MapInformation;

namespace TourPlanner.Tests;

[TestFixture]
public class StatisticsServiceTests
{
    private const string UserId = "user-1";
    private const string OtherUserId = "user-2";

    private FakeTourRepository tourRepository;
    private FakeTourLogRepository tourLogRepository;
    private StatisticsService statisticsService;

    [SetUp]
    public void SetUp()
    {
        tourRepository = new FakeTourRepository();
        tourLogRepository = new FakeTourLogRepository();
        statisticsService = new StatisticsService(
            tourRepository,
            tourLogRepository,
            NullLogger<StatisticsService>.Instance);
    }

    [Test]
    public async Task GetStatistics_WithoutTours_ShouldReturnZeroValues()
    {
        var result = await statisticsService.GetStatistics(UserId);

        Assert.That(result.TotalTours, Is.EqualTo(0));
        Assert.That(result.TotalTourLogs, Is.EqualTo(0));
        Assert.That(result.TotalDistance, Is.EqualTo(0));
        Assert.That(result.TotalTime, Is.EqualTo(0));
        Assert.That(result.AverageRating, Is.EqualTo(0));
    }

    [Test]
    public async Task GetStatistics_WithToursAndLogs_ShouldCalculateTotals()
    {
        var firstTour = CreateTour(UserId, distance: 10.125, time: 20.225);
        var secondTour = CreateTour(UserId, distance: 5.555, time: 9.444);
        tourRepository.Tours.Add(firstTour);
        tourRepository.Tours.Add(secondTour);
        tourLogRepository.TourLogs.Add(CreateTourLog(firstTour.Id, rating: 5));
        tourLogRepository.TourLogs.Add(CreateTourLog(firstTour.Id, rating: 3));
        tourLogRepository.TourLogs.Add(CreateTourLog(secondTour.Id, rating: 4));

        var result = await statisticsService.GetStatistics(UserId);

        Assert.That(result.TotalTours, Is.EqualTo(2));
        Assert.That(result.TotalTourLogs, Is.EqualTo(3));
        Assert.That(result.TotalDistance, Is.EqualTo(15.68));
        Assert.That(result.TotalTime, Is.EqualTo(29.67));
        Assert.That(result.AverageRating, Is.EqualTo(4));
    }

    [Test]
    public async Task GetStatistics_WithForeignTours_ShouldOnlyUseAuthenticatedUsersTours()
    {
        var ownedTour = CreateTour(UserId, distance: 10, time: 20);
        var foreignTour = CreateTour(OtherUserId, distance: 100, time: 200);
        tourRepository.Tours.Add(ownedTour);
        tourRepository.Tours.Add(foreignTour);
        tourLogRepository.TourLogs.Add(CreateTourLog(ownedTour.Id, rating: 5));
        tourLogRepository.TourLogs.Add(CreateTourLog(foreignTour.Id, rating: 1));

        var result = await statisticsService.GetStatistics(UserId);

        Assert.That(result.TotalTours, Is.EqualTo(1));
        Assert.That(result.TotalTourLogs, Is.EqualTo(1));
        Assert.That(result.TotalDistance, Is.EqualTo(10));
        Assert.That(result.TotalTime, Is.EqualTo(20));
        Assert.That(result.AverageRating, Is.EqualTo(5));
    }

    [Test]
    public async Task GetStatistics_WithTourWithoutRouteInfo_ShouldNotThrow()
    {
        tourRepository.Tours.Add(CreateTourWithoutRouteInfo(UserId));

        var result = await statisticsService.GetStatistics(UserId);

        Assert.That(result.TotalTours, Is.EqualTo(1));
        Assert.That(result.TotalDistance, Is.EqualTo(0));
        Assert.That(result.TotalTime, Is.EqualTo(0));
    }

    private static Tour CreateTour(string userId, double distance, double time)
    {
        return new Tour
        {
            UserId = userId,
            Title = "Test tour",
            From = "Vienna",
            To = "Graz",
            TransportType = TransportType.Bike,
            RouteInfo = JsonSerializer.Serialize(new RouteInformation
            {
                DistKm = distance,
                TimeMin = time
            })
        };
    }

    private static Tour CreateTourWithoutRouteInfo(string userId)
    {
        return new Tour
        {
            UserId = userId,
            Title = "Legacy tour",
            From = "Vienna",
            To = "Graz",
            TransportType = TransportType.Bike
        };
    }

    private static TourLog CreateTourLog(string tourId, int rating)
    {
        return new TourLog
        {
            TourId = tourId,
            TimeStamp = new DateTime(2026, 7, 6, 10, 0, 0, DateTimeKind.Utc),
            Comment = "Nice tour",
            Difficulty = Difficulty.Medium,
            TotalDistance = 10,
            TotalTime = 20,
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
            return Task.CompletedTask;
        }

        public Task Delete(TourLog obj)
        {
            TourLogs.Remove(obj);
            return Task.CompletedTask;
        }
    }
}
