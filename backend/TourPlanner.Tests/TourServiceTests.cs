using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TourPlanner.Bll;
using TourPlanner.Dal;
using TourPlanner.Models;

namespace TourPlanner.Tests;

[TestFixture]
public class TourServiceTests
{
    private string connectionString = "Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
    private ITourService TourService;
    private ITourRepository _tourRepository;
    private TourPlannerDbContext _context;
    private IOpenRouteService _IOpenRouteService;
    private User? user;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        _context = new TourPlannerDbContext(options);

        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"users\" RESTART IDENTITY CASCADE");

        _IOpenRouteService = Substitute.For<IOpenRouteService>();
        _IOpenRouteService.GetTimeAndDistance(Arg.Any<string>(),Arg.Any<string>(),Arg.Any<TransportType>()).Returns((0,0));
            
        _tourRepository = new TourRepository(_context);
        TourService = new TourService(_tourRepository,_IOpenRouteService,NullLogger<TourService>.Instance);

        user = new User { Email = "neweruser@user.us", Username = "Newest User", HashedPassword = "NewPassword" };
        _context.users.Add(user);
        await _context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _context.Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"tours\" RESTART IDENTITY CASCADE");
    }

    [Category("CreateTour")]
    [TestCase("Vienna", "Graz", TransportType.Hike)]
    [TestCase("Graz", "Salzburg", TransportType.Bike)]
    public async Task CreateTourValidInput_ShouldFindInRepository(string from, string to, TransportType type)
    {
        var tourDto = new TourDto
        {
            id = null,
            title = "Test",
            from = from,
            to = to,
            transportType = type.ToString()
        };

        var tourId = await TourService.CreateTour(user!.Id, tourDto);

        Assert.That(await _tourRepository.Read(tourId), Is.Not.Null);
        Assert.That((await _tourRepository.Read(tourId)).TransportType, Is.EqualTo(type));
    }

    [Category("CreateTour")]
    [TestCase("", "", TransportType.Hike)]
    [TestCase("Salzburg", "", TransportType.Car)]
    public async Task CreateTourInvalidInput_ShoudReturnException(string from, string to, TransportType type)
    {
        var tourDto = new TourDto
        {
            id = null,
            title = "Test",
            from = from,
            to = to,
            transportType = type.ToString()
        };

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await TourService.CreateTour(user!.Id, tourDto));
    }

    [Category("ReadTour")]
    [Test]
    public async Task ReadTourValidId_ShouldReturnTourDto()
    {
        Tour tour = new Tour
        {
            Title = "Test",
            From = "Here",
            To = "There",
            TransportType = TransportType.Car,
            UserId = user!.Id
        };
        await _tourRepository.Create(tour);

        var tourDto = await TourService.Get(tour.Id);

        Assert.That(tourDto, Is.Not.Null);
        Assert.That(tourDto.from, Is.EqualTo("Here"));
    }
    [Category("ReadTour")]
    [Test]
    public async Task ReadTourInvalidId_ShouldReturnNull()
    {
        Assert.That(await TourService.Get("001"), Is.Null);
    }

    [Category("UpdateTour")]
    [Test]
    public async Task UpdateExistingTour_ShouldReturnTrue()
    {
        Tour tour = new Tour
        {
            Title = "Test",
            From = "Here",
            To = "There",
            TransportType = TransportType.Car,
            UserId = user!.Id
        };
        await _tourRepository.Create(tour);

        var ret = await TourService.UpdateTour(tour.Id, new TourDto { id = null, from = "a", to = "b", description = "TourDescription", transportType = null });

        Assert.That(ret, Is.True);

        var retTour = await _tourRepository.Read(tour.Id);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(retTour!.Title, Is.EqualTo("Test"));
            Assert.That(retTour!.Description, Is.EqualTo("TourDescription"));
            Assert.That(retTour!.To, Is.EqualTo("b"));
        }

    }

    [Category("UpdateTour")]
    [Test]
    public async Task UpdateNonExistingTour_ShouldReturnFalse()
    {
        var ret = await TourService.UpdateTour("001", new TourDto { id = null, from = "a", to = "b", transportType = "Car" });
        Assert.That(ret, Is.False);
    }

    [Category("DeleteTour")]
    [Test]
    public async Task DeleteExistingTour_ShouldReturnTrue()
    {
        Tour tour = new Tour
        {
            Title = "Test",
            From = "Here",
            To = "There",
            TransportType = TransportType.Car,
            UserId = user!.Id
        };

        await _tourRepository.Create(tour);

        Assert.That(await _tourRepository.Read(tour.Id), Is.Not.Null);

        var ret = await TourService.Remove(tour.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.True);
            Assert.That(await _tourRepository.Read(tour.Id), Is.Null);
        }

    }

    [Category("DeleteTour")]
    [Test]
    public async Task DeleteNonExistingTour_ShouldReturnFalse()
    {
        Tour tour = new Tour
        {
            Title = "Test",
            From = "Here",
            To = "There",
            TransportType = TransportType.Car,
            UserId = user!.Id
        };

        // Tour Is Not Created

        var ret = await TourService.Remove(tour.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(await _tourRepository.Read(tour.Id), Is.Null);
        }


    }

    [Category("ReadToursFromQuery")]
    [Test]
    public async Task CreateTourViaService_ShouldAppearInGetToursForSameUser()
    {
        var tourDto = new TourDto
        {
            id = null,
            title = "CreatedByService",
            from = "A",
            to = "B",
            transportType = "Car"
        };

        await TourService.CreateTour(user!.Id, tourDto);

        var retList = await TourService.GetTours(user!.Id);

        Assert.That(retList, Is.Not.Null);
        Assert.That(retList!.Count(), Is.EqualTo(1));
    }

    [Category("ReadToursFromQuery")]
    [Test]
    public async Task ReadToursNoQuery_ShouldReturnAllTours()
    {
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour1", From = "A", To = "B", Description = "One Description"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour2", From = "B", To = "C", Description = "A Tour"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour3", From = "A", To = "C", Description = "The Third"});

        var retList = await TourService.GetTours(user!.Id);

        Assert.That(retList!.Count(),Is.EqualTo(3));
    }

    [Category("ReadToursFromQuery")]
    [TestCase("C",2)]
    [TestCase("Tour",3)]
    [TestCase("1",1)]
    public async Task ReadToursWithQuery_ShouldReturnAllTours(string query, int count)
    {
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour1", From = "A", To = "B", Description = "One Description"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour2", From = "B", To = "C", Description = "A Tour"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour3", From = "A", To = "C", Description = "The Third"});

        var retList = await TourService.GetTours(user!.Id, query);

        Assert.That(retList!.Count(),Is.EqualTo(count));
    }

    [Category("ReadToursFromQuery")]
    [Test]
    public async Task ReadToursInvalidUserId_ShouldReturnNull()
    {
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour1", From = "A", To = "B", Description = "One Description"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour2", From = "B", To = "C", Description = "A Tour"});
        await _tourRepository.Create(new Tour{UserId = user!.Id,Title = "Tour3", From = "A", To = "C", Description = "The Third"});

        var retList = await TourService.GetTours("001");

        Assert.That(retList,Is.Empty);
    }
}