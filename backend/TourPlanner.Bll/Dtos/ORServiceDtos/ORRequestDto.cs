using TourPlanner.Models;
using TourPlanner.Models.MapInformation;

namespace TourPlanner.Bll.Dtos;

public class ORServiceRequestDto
{
    public Coordinates start {get; set;}
    public Coordinates dest {get; set;}
    public TransportType profile {get; set;}
}