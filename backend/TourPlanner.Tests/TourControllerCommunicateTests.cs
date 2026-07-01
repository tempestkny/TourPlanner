
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Api.Controllers;
using TourPlanner.Bll;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
}

[TestFixture]
public class TourControllerAuthTest
{
    private ApiFactory _factory;
    private HttpClient _client;

    [SetUp]
    public async Task SetUp()
    {
        _factory = new ApiFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public async Task TearDown()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [Test]
    public async Task TestAllUnAuthorized_ShouldReturnUnAuthorized()
    {
        var response = await _client.GetAsync("/api/tour");
        var expected = HttpStatusCode.Unauthorized;
        Assert.That(response.StatusCode, Is.EqualTo(expected));

    }
}



[TestFixture]
public class TourControllerCommunicationTests 
{
    private TourController tourController;
    private ITourRepository _tourRepository;
    private TourPlannerDbContext _context;
    private User? user;
 
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";

        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        _context = new TourPlannerDbContext(options);

        _tourRepository = new TourRepository(_context);
        tourController = new TourController(new TourService(_tourRepository));
        

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
    public async Task TestAllUnAuthorized_ShouldReturnUnauthorized()
    {
        
    }

    [Test]
    public async Task CreateTourValidContent_ShouldReturnCreated()
    {
        
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