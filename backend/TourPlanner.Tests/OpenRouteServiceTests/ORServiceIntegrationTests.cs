using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TourPlanner.Bll;
using TourPlanner.Bll.Dtos;
using TourPlanner.Models;
using TourPlanner.Models.MapInformation;
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
            ApiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6IjhhY2Q3M2QwYTQ2OTRkYWY5OWM2NGZkM2NhODFjMGQ5IiwiaCI6Im11cm11cjY0In0="
        });
        openRouteService = new OpenRouteService(httpClient, options, new NullLogger<OpenRouteService>());
    }

    [TestCase("Währinger Straße 21", 16.36, 48.22)]
    [TestCase("Issy-les-Moulineaux", 2.26, 48.82)]
    public async Task GetCoordinatesValidPlace_ShouldReturnCoordinates(string place, double exLong, double exLat)
    {
        var coords = await openRouteService.GetCoordinates(place);

        Assert.That(coords, Is.Not.Null);
        Assert.That(coords.Lon, Is.InRange(exLong - 0.1, exLong + 0.1));
        Assert.That(coords.Lat, Is.InRange(exLat - 0.1, exLat + 0.1));

    }

    public async Task GetTimeAndDistanceValidRoute_ShouldReturnTimeAndDistance()
    {
        var vienna = new Coordinates { Lon = 16.348388, Lat = 48.198674 };
        var graz = new Coordinates { Lon = 15.432739, Lat = 47.065852 };
        var profile = TransportType.Car;
        var expectedTimeMin = 10;
        var expectedDistKm = 10;

        var request = new ORServiceRequestDto() { start = vienna, dest = graz, profile = profile };
        var response = await openRouteService.GetRouteInformation(request);

        Assert.That(response.DistKm, Is.GreaterThan(expectedDistKm));
        Assert.That(response.TimeMin, Is.GreaterThan(expectedTimeMin));
        Assert.That(response.Route!.Count(), Is.GreaterThan(1));

    }
}