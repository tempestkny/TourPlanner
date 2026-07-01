using TourPlanner.Bll;
using TourPlanner.Models;
namespace TourPlanner.Tests;

[TestFixture]
public class OpenRouteServiceTests
{
    

    [TestCase("Währinger Straße 21",16.36,48.22)]
    [TestCase("Issy-les-Moulineaux",2.26,48.82)]
    public async Task GetCoordinatesValidPlace_ShouldReturnCoordinates(string place, double exLong, double exLat)
    {
        
    }

    [TestCase("","",TransportType.Car,0,0)]
    public async Task GetTimeAndDistanceValidRoute_ShouldReturnTimeAndDistance(string start, string end, TransportType profile, double exTim, double exDist)
    {
        
    }
}