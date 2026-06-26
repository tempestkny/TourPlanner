using Microsoft.EntityFrameworkCore;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class TourRepositoryTest
{
    string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
    ITourRepository TourRepository;
    TourPlannerDbContext context;

    User? user;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
                        .UseNpgsql(connectionString)
                        .Options;
        context = new TourPlannerDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        TourRepository = new TourRepository(context);

        user = new User { Email = "newuser@user.us", Username = "New User", HashedPassword = "NewPassword" };
        context.users.Add(user);
        await context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");
        context?.Dispose();
    }

    [Test]
    public async Task CreateTour_ShouldFindInDataBase()
    {
        var tour = new Tour()
        {
            From = "Here",
            To = "There",
            Title = "MyTour",
            TourDescription = "MyTourDescription",
            TransportType = TransportType.Car,
            User = user
        };

        await TourRepository.Create(tour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.tours.FirstOrDefault(t => t.From == "Here"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.To == "There"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.Title == "MyTour"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.TourDescription == "MyTourDescription"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.TransportType == TransportType.Car), Is.Not.Null);
        }
    }

    [Test]
    public async Task FindTourInDataBase()
    {
        var tour = new Tour
        {
            From = "Vienna",
            To = "Salzburg",
            Title = "LongTour",
            TourDescription = "A Very Long Tour",
            TransportType = TransportType.Bike,
            User = user
        };

        context.tours.Add(tour);
        context.SaveChanges();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TourRepository.Read(tour.Id), Is.Not.Null);
        }

    }

    [Test]
    public async Task DeleteTour_ShouldReturnNull()
    {
        var tour = new Tour
        {
            From = "NotAPlace",
            To = "NotAPlace",
            Title = "NoTitle",
            TourDescription = "NoDescription",
            TransportType = TransportType.Hike,
            User = user
        };

        context.tours.Add(tour);
        context.SaveChanges();

        await TourRepository.Delete(tour);

        Assert.That(context.tours.Find(tour.Id), Is.Null);
    }
}