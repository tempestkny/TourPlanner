namespace TourPlanner.Tests;

using Microsoft.EntityFrameworkCore;
using TourPlanner.Dal;

public class DBCOntextSetupTest
{
    string connectionString ="Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
    TourPlannerDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
        .UseNpgsql(connectionString)
        .Options;
        context = new TourPlannerDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        context?.Dispose();
    }

    [Test]
    public void TestIfDBContextIsConnectable()
    {
        bool canConnect = context.Database.CanConnect();
        Assert.That(canConnect, Is.True);
    }

    [Test]
    public void EnsureSchemaCreated()
    {
        Assert.That(context.Database.EnsureCreated(),Is.True);
    }

}
