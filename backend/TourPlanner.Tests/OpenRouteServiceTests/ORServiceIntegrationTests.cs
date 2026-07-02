using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TourPlanner.Bll;
using TourPlanner.Models;
namespace TourPlanner.Tests.ORSTest;

[TestFixture]
public class OpenRouteServiceIntegrationTests
{
    OpenRouteService openRouteService;

    [OneTimeSetUp]
    public async Task SetupOnce()
    {
        var httpClient = new HttpClient
        {
          BaseAddress = new Uri("https://api.openrouteservice.org/")
        };

        var options = Options.Create(new OpenRouteServiceOptions
        {
           ApiKey = Environment.GetEnvironmentVariable("ORS_API_KEY") ?? throw new Exception("ORS_API_KEY not set")
        });
        openRouteService = new OpenRouteService(httpClient, options,new NullLogger<OpenRouteService>());
    }

    [TestCase("Währinger Straße 21",16.36,48.22)]
    [TestCase("Issy-les-Moulineaux",2.26,48.82)]
    public async Task GetCoordinatesValidPlace_ShouldReturnCoordinates(string place, double exLong, double exLat)
    {
        var coords = await openRouteService.GetCoordinates(place);

        Assert.That(coords,Is.Not.Null);
        Assert.That(coords.Value.lon,Is.InRange(exLong - 0.1, exLong + 0.1));
        Assert.That(coords.Value.lat,Is.InRange(exLat - 0.1, exLat + 0.1));
        
    }

    [TestCase("Wien","Graz",TransportType.Car,1,1000)]
    public async Task GetTimeAndDistanceValidRoute_ShouldReturnTimeAndDistance(string start, string end, TransportType profile, double exTim, double exDist)
    {
        var (distM,timeMin) = await openRouteService.GetTimeAndDistance(start,end,profile);
        Assert.That(distM,Is.GreaterThan(exDist));
        Assert.That(timeMin,Is.GreaterThan(exTim));

    }
}