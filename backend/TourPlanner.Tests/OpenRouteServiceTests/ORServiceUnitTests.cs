using System.Net;
using System.Text;
using TourPlanner.Bll;
using TourPlanner.Models;
using TourPlanner.Tests.MockService;
namespace TourPlanner.Tests.ORSTest;

[TestFixture]
public class OpenRouteServiceUnitTests
{
    OpenRouteService openRouteService;

    [TestCase("Währinger Straße 21", 16.36, 48.22)]
    [TestCase("Issy-les-Moulineaux", 2.26, 48.82)]
    public async Task GetCoordinatesValidPlace_ShouldReturnCoordinates(string place, double exLong, double exLat)
    {

    }

    [TestCase("", "", TransportType.Car, 0, 0)]
    public async Task GetTimeAndDistanceValidRoute_ShouldReturnTimeAndDistance(string start, string end, TransportType profile, double exTim, double exDist)
    {

    }


    private HttpMessageHandler MockHandler(double lon, double lat, double tim, double dist)
    {
        return new MockHttpMessageHandler(req =>
    {
        if (req.RequestUri!.AbsolutePath.Contains("geocode"))
        {
            var json = """
            {
                "features":[
                {
                    "geometry": {
                        "coordinates" : [16.3738, 48.2082]
                    }
                }]
            }
            """;

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        }

    if (req.RequestUri!.AbsolutePath.Contains("directions"))
    {
        var json = """
        {
            "routes": [
                {
                    "summary": {
                        "distance": 12345.0,
                        "duration": 1800.0
                    }
                }
            ]
        }
        """;

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    return new HttpResponseMessage(HttpStatusCode.NotFound);

    });
    }
}