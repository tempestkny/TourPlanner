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
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");

        TourRepository = new TourRepository(context);

        user = new User { Email = "newuser@user.us", Username = "New User", HashedPassword = "NewPassword" };
        context.users.Add(user);
        await context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        context?.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");
    }

    [Test]
    public async Task CreateTour_ShouldFindInDataBase()
    {
        var tour = new Tour()
        {
            From = "Here",
            To = "There",
            Title = "MyTour",
            Description = "MyTourDescription",
            TransportType = TransportType.Car,
            UserId = user.Id
        };

        await TourRepository.Create(tour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.tours.FirstOrDefault(t => t.From == "Here"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.To == "There"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.Title == "MyTour"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.Description == "MyTourDescription"), Is.Not.Null);
            Assert.That(context.tours.FirstOrDefault(t => t.TransportType == TransportType.Car), Is.Not.Null);
        }
    }
    
    [Test]
    public async Task FindTourInDataBase()
    {
        var tour = new Tour
        {
            From = "Here",
            To = "There",
            Title = "Long",
            Description = "Very Long",
            TransportType = TransportType.Bike,
            UserId = user.Id
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
            Description = "NoDescription",
            TransportType = TransportType.Hike,
            UserId = user.Id
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
            Description = "NoDescription",
            TransportType = TransportType.Hike,
            UserId = user.Id
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

        Assert.That(newTour, Is.Not.Null);
        Assert.That(newTour!.Title, Is.EqualTo("Place"));
        Assert.That(newTour.Description, Is.EqualTo("NoDescription"));
        Assert.That(newTour.From, Is.EqualTo("Place2"));
        Assert.That(newTour.To, Is.EqualTo("Place1"));
        Assert.That(newTour.TransportType, Is.EqualTo(TransportType.Car));

    }

    [TestCase("My", 1, 1)]
    [TestCase("favorite", 1, 1)]
    [TestCase("Vienna", 1, 1)]
    [TestCase("Salzburg", 1,2)]
    [TestCase("Hike", 1, 1)]
    [TestCase("10", 1, 2)]
    [TestCase("5", 1, 2)]
    [TestCase("Good", 2, 1)]
    [TestCase("Bike", 2, 1)]
    [TestCase("Linz", 2, 2)]
    [TestCase("12", 2, 1)]
    [TestCase("Your", 3, 1)]
    [TestCase("Graz", 3, 1)]
    [TestCase("Car", 3, 1)]
    [TestCase("Fire", 0, 0)]
    public async Task ReadFromQuery_ShouldMatchTourFields(string query, int expectedTourIndex,int count)
    {
        var empty = new Tour(){From = "",To = ""};
        var tour1 = new Tour
        {
            From = "Vienna",
            To = "Salzburg",
            Title = "My Tour",
            Description = "My favorite Tour",
            TransportType = TransportType.Hike,
            Distance = 10,
            Time = 5,
            UserId = user.Id
        };

        var tour2 = new Tour
        {
            From = "Salzburg",
            To = "Linz",
            Title = "Good Tour",
            Description = "Bike Tour",
            TransportType = TransportType.Bike,
            UserId = user.Id,
            Distance = 5,
            Time = 12
        };

        var tour3 = new Tour
        {
            From = "Linz",
            To = "Graz",
            Title = "Your Tour",
            Description = "No Hiking Tour",
            TransportType = TransportType.Car,
            UserId = user.Id,
            Distance = 0,
            Time = 10
        };

        AddTour(tour1);
        AddTour(tour2);
        AddTour(tour3);

        var result = (await TourRepository.ReadFromQuery(user!.Id, query)).ToList();

        Assert.That(result, Has.Count.EqualTo(count));
        Assert.That(result.FirstOrDefault(empty)!.Id, Is.EqualTo(expectedTourIndex switch
        {
            0 => empty.Id,
            1 => tour1.Id,
            2 => tour2.Id,
            _ => tour3.Id
        }));
    }

    [Test]
    public async Task ReadFromQuery_WithEmptyQuery_ShouldReturnAllToursForUser()
    {
        var tour1 = new Tour
        {
            From = "Vienna",
            To = "Salzburg",
            Title = "My Tour",
            Description = "My favorite Tour",
            TransportType = TransportType.Hike,
            Distance = 10,
            Time = 5,
            UserId = user!.Id
        };

        var tour2 = new Tour
        {
            From = "Salzburg",
            To = "Linz",
            Title = "Good Tour",
            Description = "Bike Tour",
            TransportType = TransportType.Bike,
            UserId = user.Id,
            Distance = 5,
            Time = 12
        };

        AddTour(tour1);
        AddTour(tour2);

        var result = (await TourRepository.ReadFromQuery(user!.Id, string.Empty)).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(t => t.Id), Is.EquivalentTo([tour1.Id, tour2.Id]));
    }


    /// HelperFunctions
    
    void AddTour(Tour tour)
    {
        context.tours.Add(tour);
        context.SaveChanges();
    }
    Tour? FindTour(string id) => context.tours.Find(id);

}
