using TourPlanner.Models;
using TourPlanner.Models.MapInformation;
namespace TourPlanner.Bll.Dtos;

public class UpdateTourDto
{
    public string? Title {get; set;}
    public string? Description {get; set;}
    public string? From {get; set;}
    public string? To {get; set;}
    public TransportType? TransportType {get; set;}

    public RouteInformation? Route {get; set;}
}