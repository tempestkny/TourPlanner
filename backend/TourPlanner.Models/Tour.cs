using System.Text.Json.Serialization;

namespace TourPlanner.Models;

public class Tour
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public required TransportType TransportType { get; set; }

    public string? RouteInfo {get; set;}

    // Navigation
    public string? UserId { get; set; }
    public User? User {get; set; }

    public ICollection<TourLog> TourLogs {get; set;} = new List<TourLog>();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransportType
{
    Car,
    Bike,
    Hike
}
