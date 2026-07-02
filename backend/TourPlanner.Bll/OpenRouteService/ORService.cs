using System.Net.Http.Json;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourPlanner.Models;

namespace TourPlanner.Bll;

public class OpenRouteService : IOpenRouteService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<OpenRouteService> _logger;

    public OpenRouteService(HttpClient http, IOptions<OpenRouteServiceOptions> options, ILogger<OpenRouteService> logger)
    {
        _http = http;
        _apiKey = options.Value.ApiKey;
        _logger = logger;
    }
    public async Task<(double lon, double lat)?> GetCoordinates(string place)
    {
        var url = $"/geocode/search?api_key={_apiKey}&text={place}";
        var response = await _http.GetFromJsonAsync<GeocodeResponse>(url);
        
        var first = response?.Features?.FirstOrDefault();
        if(first == null){
            _logger.LogInformation("No Coordinates could be extracted from location: {place}", place);
            return null;
        }

        var coords = first.Geometry.Coordinates;
        _logger.LogInformation("Returning Coordinates of Location {place}: {coords}",place,coords);
        return (coords[0],coords[1]);
    }

    public async Task<(double distM, double timeMin)> GetTimeAndDistance(string start, string dest, TransportType profile)
    {
        var queryStart = await GetCoordinates(start);
        var queryEnd = await GetCoordinates(dest);

        if(queryStart == null || queryEnd == null){
            //_logger.LogInformation("");
            throw new ArgumentException($"route {queryStart} -> {queryEnd} could not be found");
            }

        var pathProfile = profile switch
        {
            TransportType.Car => "driving-car",
            TransportType.Bike => "cycling-regular",
            TransportType.Hike => "foot-hiking",
            _ => "driving-car"
        };

        var body = new
        {
            coordinates = new[]
            {
                new[] {queryStart.Value.lon,queryStart.Value.lat},
                new[] {queryEnd.Value.lon, queryEnd.Value.lat}
            }
        };

        var response = await _http.PostAsJsonAsync($"/v2/directions/{pathProfile}?api_key={_apiKey}",body);
        var result = await response.Content.ReadFromJsonAsync<DirectionsResponse>();

        var summary = result?.Routes?.FirstOrDefault()?.directionsSummary;
        if(summary == null)
        {
            _logger.LogInformation("The route from {start} to {dest} by {profile} could not be calculated.",start,dest,profile);
            return (0,0);
        }

        return (summary.Distance,summary.Duration / 60.0);
    }
}

public class GeocodeResponse{ public List<GeocodeFeature>? Features {get; set;}}
public class GeocodeFeature{ public GeoCodeGeometry? Geometry {get; set;}}
public class GeoCodeGeometry{ public List<double>? Coordinates {get;set;}}

public class DirectionsResponse{ public List<DirectionsRoute>? Routes {get; set;}}
public class DirectionsRoute{ public DirectionsSummary? directionsSummary {get; set;}}
public class DirectionsSummary{ 
    public double Distance{get;set;}
    public double Duration{get;set;}
}