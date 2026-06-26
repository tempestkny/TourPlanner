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
        //await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");
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

        AddTour(tour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(FindTour(tour.Id), Is.Not.Null);
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

        AddTour(tour);

        await TourRepository.Delete(tour);

        Assert.That(FindTour(tour.Id), Is.Null);
    }

    [Test]
    public async Task UpdateTour_ShouldReturnUpdatedTour()
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

        AddTour(tour);

        await TourRepository.Update(tour.Id, new Tour
        {
            Title = "Place",
            To = "Place1",
            From = "Place2",
            TransportType = TransportType.Car
        });

        var newTour = FindTour(tour.Id);

        Assert.That(newTour.Title, Is.EqualTo("Place"));
        Assert.That(newTour.TourDescription, Is.EqualTo("NoDescription"));
        Assert.That(newTour.From, Is.EqualTo("Place2"));
        Assert.That(newTour.To, Is.EqualTo("Place1"));
        Assert.That(newTour.TransportType, Is.EqualTo(TransportType.Car));

    }

    [TestCase("My")]
    [TestCase("Tour")]
    [TestCase("Fire")]
    public async Task FindListOfToursByQuery_ShouldReturnTours(string value)
    {
        
    }


    /// HelperFunctions
    
    void AddTour(Tour tour)
    {
        context.tours.Add(tour);
        context.SaveChanges();
    }
    Tour? FindTour(string id) => context.tours.Find(id);

}