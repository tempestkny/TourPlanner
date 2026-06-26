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

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
        .UseNpgsql(connectionString)
        .Options;
        context = new TourPlannerDbContext(options);

        TourRepository = new TourRepository(context);

        
    }

    [TearDown]
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
            
        };

        await TourRepository.Create(tour);

        using (Assert.EnterMultipleScope())
        {

        }

    }

    [Test]
    public async Task FindTourInDataBase()
    {
        var tour = new Tour
        {

        };

        context.tours.Add(tour);
        context.SaveChanges();

        using (Assert.EnterMultipleScope())
        {

        }

    }

    [Test]
    public async Task DeleteTour_ShouldReturnNull()
    {
        var tour = new Tour
        {

        };

        context.tours.Add(tour);
        context.SaveChanges();

        await TourRepository.Delete(tour);

        Assert.That(context.users.Find(tour.Id),Is.Null);
    }
}