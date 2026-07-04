using System.ComponentModel.DataAnnotations;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class ImportExportServiceTests
{
    private const string UserId = "user-1";
    private const string OtherUserId = "user-2";

    private FakeTourRepository tourRepository;
    private FakeTourLogRepository tourLogRepository;
    private ImportExportService importExportService;

    [SetUp]
    public void SetUp()
    {
        tourRepository = new FakeTourRepository();
        tourLogRepository = new FakeTourLogRepository();
        importExportService = new ImportExportService(tourRepository, tourLogRepository);
    }

    [Test]
    public async Task Export_WithToursAndLogs_ShouldReturnOwnedToursWithLogs()
    {
        var tour = CreateTour(UserId);
        tourRepository.Tours.Add(tour);
        tourLogRepository.TourLogs.Add(CreateTourLog(tour.Id, rating: 5));
        tourLogRepository.TourLogs.Add(CreateTourLog(tour.Id, rating: 3));

        var result = await importExportService.Export(UserId);

        Assert.That(result.Tours, Has.Count.EqualTo(1));
        var exportedTour = result.Tours.Single();
        Assert.That(exportedTour.Id, Is.EqualTo(tour.Id));
        Assert.That(exportedTour.TourLogs, Has.Count.EqualTo(2));
        Assert.That(exportedTour.TourLogs.Select(log => log.Rating), Is.EquivalentTo(new[] { 5, 3 }));
    }

    [Test]
    public async Task Export_WithForeignTours_ShouldNotReturnForeignUserData()
    {
        tourRepository.Tours.Add(CreateTour(UserId));
        tourRepository.Tours.Add(CreateTour(OtherUserId));

        var result = await importExportService.Export(UserId);

        Assert.That(result.Tours, Has.Count.EqualTo(1));
        Assert.That(result.Tours.Single().Title, Is.EqualTo("Owned tour"));
    }

    [Test]
    public async Task Import_WithValidPayload_ShouldCreateToursAndTourLogs()
    {
        var importData = CreateImportData();

        var importedTours = await importExportService.Import(UserId, importData);

        Assert.That(importedTours, Is.EqualTo(1));
        Assert.That(tourRepository.Tours, Has.Count.EqualTo(1));
        Assert.That(tourLogRepository.TourLogs, Has.Count.EqualTo(1));
        Assert.That(tourLogRepository.TourLogs.Single().TourId, Is.EqualTo(tourRepository.Tours.Single().Id));
    }

    [Test]
    public async Task Import_WithValidPayload_ShouldAssignAuthenticatedUserId()
    {
        var importData = CreateImportData();

        await importExportService.Import(UserId, importData);

        Assert.That(tourRepository.Tours.Single().UserId, Is.EqualTo(UserId));
    }

    [Test]
    public void Import_WithInvalidPayload_ShouldThrowValidationException()
    {
        var importData = new ImportTourDataDto
        {
            Tours =
            [
                new ImportTourDto
                {
                    From = null!,
                    To = "Graz",
                    TransportType = TransportType.Bike,
                    Distance = 100,
                    Time = 60
                }
            ]
        };

        Assert.ThrowsAsync<ValidationException>(() => importExportService.Import(UserId, importData));
    }

    private static Tour CreateTour(string userId)
    {
        return new Tour
        {
            UserId = userId,
            Title = userId == UserId ? "Owned tour" : "Foreign tour",
            Description = "Description",
            From = "Vienna",
            To = "Graz",
            TransportType = TransportType.Bike,
            Distance = 100,
            Time = 60
        };
    }

    private static TourLog CreateTourLog(string tourId, int rating)
    {
        return new TourLog
        {
            TourId = tourId,
            TimeStamp = new DateTime(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc),
            Comment = "Nice tour",
            Difficulty = Difficulty.Medium,
            TotalDistance = 100,
            TotalTime = 60,
            Rating = rating,
            Tour = null!
        };
    }

    private static ImportTourDataDto CreateImportData()
    {
        return new ImportTourDataDto
        {
            Tours =
            [
                new ImportTourDto
                {
                    Title = "Imported tour",
                    Description = "Imported description",
                    From = "Vienna",
                    To = "Graz",
                    TransportType = TransportType.Bike,
                    Distance = 100,
                    Time = 60,
                    TourLogs =
                    [
                        new ImportTourLogDto
                        {
                            TimeStamp = new DateTime(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc),
                            Comment = "Imported log",
                            Difficulty = Difficulty.Easy,
                            TotalDistance = 100,
                            TotalTime = 60,
                            Rating = 5
                        }
                    ]
                }
            ]
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
