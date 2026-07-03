namespace TourPlanner.Models;

public class TourDto
{
    public string? id {get; set;}
    public string? title {get; set;}
    public string? description {get; set;}
    public string from {get; set;}
    public string to {get; set;}
    public string transportType {get; set;}

    public double? distance {get; set;}
    public double? time {get; set;}
}