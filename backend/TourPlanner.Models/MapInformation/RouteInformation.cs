namespace TourPlanner.Models.MapInformation;

public class RouteInformation
{
    public double? TimeMin { get; set; }
    public double? DistKm { get; set; }
    public List<Coordinates>? Route { get; set; }
}