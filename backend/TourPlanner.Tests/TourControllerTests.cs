
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Api.Controllers;
using TourPlanner.Bll;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class TourControllerTests : WebApplicationFactory<Program>
{
    private TourController tourController;
    private ITourRepository _tourRepository;
    private TourPlannerDbContext _context;
    private User? user;
 
    [OneTimeSetUp]
    public async void OneTimeSetUp()
    {
        string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";

        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        _context = new TourPlannerDbContext(options);

        _tourRepository = new TourRepository(_context);
        tourController = new TourController(new TourService(_tourRepository));
        

        user = new User { Email = "newuser@user.us", Username = "New User", HashedPassword = "NewPassword" };
        _context.users.Add(user);
        await _context.SaveChangesAsync();

    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void TestAllUnAuthorized_ShouldReturnUnauthorized()
    {
        
    }

    [Test]
    public void CreateTourValidContent_ShouldReturnCreated()
    {
        
    }

    [Test]
    public void CreateTourInValidContent_ShouldReturnBadRequest()
    {
        
    }

    [Test]
    public void GetAllFromUserToken_ShouldReturnOKUserTours()
    {
        
    }

    [Test]
    public void GetAllFromQuery_ShouldReturnOKUserToursFittingToQuery()
    {
        
    }

    [Test]
    public void GetTourFromExistingId_ShouldReturnOKWithTour()
    {
        
    }

    [Test]
    public void GetDeletedTour_ShouldReturnNotFound()
    {
        
    }

    [Test]
    public void UpdateTourValidID_ShouldReturnNoContent()
    {
        
    }
    [Test]
    public void UpdateTourInValidID_ShouldReturnNotFound()
    {
        
    }
    [Test]
    public void DeleteTour_ShouldReturnNoContent()
    {
        
    }
        [Test]
    public void DeleteNonExistantTour_ShouldReturnNotFound()
    {
        
    }
}