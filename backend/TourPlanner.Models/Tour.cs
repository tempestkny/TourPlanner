namespace TourPlanner.Models;

public class Tour
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public TransportType? TransportType { get; set; }

    // Navigation
    public string? UserId { get; set; }
    public User? User {get; set; }

    public ICollection<TourLog> TourLogs {get; set;} = new List<TourLog>();

    // TourData

    public double Distance { get; set; }
    public double Time { get; set; }
}

public enum TransportType
{
    Car,
    Bike,
    Hike
}
