namespace TourPlanner.Models;
public class Tour
{
    public required string Id { get; set; }
    public required string userId { get; set; }
    public string? title { get; set; }
    public string? tourDescription { get; set; }
    public required string from { get; set; }
    public required string to { get; set; }
    public TransportType? transportType { get; set; }

    // TourData

    public double distance { get; set; }
    public double time { get; set; }
    public string? mapJSON { get; set; } // Whatever is required for the Map
}

public enum TransportType
{
    Car,
    Bike,
    Hike
}
