namespace TourPlanner.Models;

public class TourDto
{
    public required string? id;
    public string? title;
    public string? description;
    public required string from;
    public required string to;
    public required TransportType? transportType;

    public double? distance;
    public double? time;
}