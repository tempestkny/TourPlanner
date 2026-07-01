using Microsoft.Extensions.Options;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class OpenRouteService : IOpenRouteService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public OpenRouteService(HttpClient http, IOptions<OpenRouteServiceOptions> options)
    {
        _http = http;
        _apiKey = options.Value.ApiKey;
    }
    public async Task<(double lat, double lon)?> GetCoordinates(string place)
    {
        var url = $"/geocode/search?api_key={_apiKey}&text={place}";

    }

    public async Task<(double distM, double timeMin)> GetTimeAndDistance(string start, string dest, TransportType profile)
    {
        var queryStart = GetCoordinates(start);
        var queryEnd = GetCoordinates(dest);
        var pathProfile = profile switch
        {
            TransportType.Car => "driving-car",
            TransportType.Bike => "cycling-regular",
            TransportType.Hike => "foot-hiking",
            _ => "driving-car"
        };

        

        return (0,0);
    }
}