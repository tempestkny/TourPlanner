
using NSubstitute;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Api.Controllers;
using TourPlanner.Bll;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;


[TestFixture]
public class TourControllerTests 
{
    private TourController tourController;
    private ITourRepository _tourRepository;
    private TourPlannerDbContext _context;
    private User? user;

    private IOpenRouteService _IOpenRouteService;
 
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
        _IOpenRouteService = Substitute.For<IOpenRouteService>();

        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new TourPlannerDbContext(options);
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"users\" RESTART IDENTITY CASCADE");

        _tourRepository = new TourRepository(_context);
        tourController = new TourController(new TourService(_tourRepository,_IOpenRouteService));
        

        user = new User { Email = "myuser@user.you", Username = "Newly User", HashedPassword = "NewlyPasswordy" };
        _context.users.Add(user);
        await _context.SaveChangesAsync();

    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task CreateTourValidContent_ShouldReturnCreated()
    {
        TourDto tour = new TourDto
        {
            id = null,
            from = "here",
            to = "there",
            transportType = TransportType.Car
        };
    }

    [Test]
    public async Task CreateTourInValidContent_ShouldReturnBadRequest()
    {
        
    }

    [Test]
    public async Task GetAllFromUserToken_ShouldReturnOKUserTours()
    {
        
    }

    [Test]
    public async Task GetAllFromQuery_ShouldReturnOKUserToursFittingToQuery()
    {
        
    }

    [Test]
    public async Task GetTourFromExistingId_ShouldReturnOKWithTour()
    {
        
    }

    [Test]
    public async Task GetDeletedTour_ShouldReturnNotFound()
    {
        
    }

    [Test]
    public async Task UpdateTourValidID_ShouldReturnNoContent()
    {
        
    }
    [Test]
    public async Task UpdateTourInValidID_ShouldReturnNotFound()
    {
        
    }
    [Test]
    public async Task DeleteTour_ShouldReturnNoContent()
    {
        
    }
        [Test]
    public async Task DeleteNonExistantTour_ShouldReturnNotFound()
    {
        
    }
}