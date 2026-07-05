using System.Net.Http.Json;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourPlanner.Bll.Dtos;
using TourPlanner.Models;
using TourPlanner.Models.MapInformation;

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
    public async Task<Coordinates> GetCoordinates(string place)
    {
        var url = $"/geocode/search?api_key={_apiKey}&text={place}";
        var response = await _http.GetFromJsonAsync<GeocodeResponse>(url);

        if (response == null)
        {
            throw new Exception($"No Coordinates could be extracted from location: {place}");
        }

        var first = response?.Features?.FirstOrDefault();


        var coords = first.Geometry.Coordinates;
        _logger.LogInformation("Returning Coordinates of Location {place}: {coords}", place, coords);
        return new Coordinates { Lon = coords[0], Lat = coords[1] };
    }
    public async Task<RouteInformation> GetRouteInformation(ORServiceRequestDto request)
    {
        var start = request.start;
        var dest = request.dest;
        var profile = request.profile;

        var pathProfile = profile switch
        {
            TransportType.Car => "driving-car",
            TransportType.Bike => "cycling-regular",
            TransportType.Hike => "foot-hiking",
            _ => "driving-car"
        };

        var url = FormattableString.Invariant($"/v2/directions/{pathProfile}?api_key={_apiKey}&start={start.Lon},{start.Lat}&end={dest.Lon},{dest.Lat}");
        _logger.LogInformation("Sending Request to ORS: {url}", _http.BaseAddress + url);

        try
        {
            var response = await _http.GetFromJsonAsync<DirectionsResponse>(url);
            var feature = response?.Features?.FirstOrDefault();
            if (feature == null)
            {
                throw new ArgumentException($"The route from ({start.Lon},{start.Lat}) to ({dest.Lon},{dest.Lat}) by {profile} could not be calculated.");
            }

            return new RouteInformation
            {
                TimeMin = feature.Properties.Summary.Duration / 60,
                DistKm = feature.Properties.Summary.Distance / 1000,
                Route = ToCoordinates(feature.Geometry)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ORS request failed at URL: {url}", url);
            throw;
        }
    }

    private List<Coordinates> ToCoordinates(DirectionsGeometry geoCodeGeometry)
    {
        if (geoCodeGeometry.Coordinates == null)
            throw new Exception("ORService: The RouteCoordinates do not exist.");

        return geoCodeGeometry.Coordinates.Select(coord => new Coordinates
        {
            Lon = coord[0],
            Lat = coord[1]
        }).ToList();
    }
}

public class GeocodeResponse { public List<GeocodeFeature>? Features { get; set; } }
public class GeocodeFeature { public GeoCodeGeometry? Geometry { get; set; } }
public class GeoCodeGeometry { public List<double>? Coordinates { get; set; } }

public class DirectionsResponse { public List<DirectionsFeature>? Features { get; set; } }
public class DirectionsFeature
{
    public DirectionsGeometry? Geometry { get; set; }
    public DirectionsProperty? Properties { get; set; }
}
public class DirectionsProperty { public DirectionsSummary? Summary { get; set; } }
public class DirectionsGeometry { public List<List<double>>? Coordinates { get; set; } }
public class DirectionsSummary
{
    public double Distance { get; set; }
    public double Duration { get; set; }
}
