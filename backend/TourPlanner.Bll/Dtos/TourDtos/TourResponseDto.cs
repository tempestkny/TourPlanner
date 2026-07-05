using TourPlanner.Models;
using TourPlanner.Models.MapInformation;
namespace TourPlanner.Bll.Dtos;

public class TourResponseDto
{
    public required string Id {get; set;}
    public required string Title {get; set;}
    public string? Description {get; set;}
    public required string From {get; set;}
    public required string To {get; set;}
    public required TransportType TransportType {get; set;}

    public required RouteInformation route {get; set;}
}