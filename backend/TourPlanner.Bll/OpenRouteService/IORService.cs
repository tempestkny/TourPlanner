using TourPlanner.Bll.Dtos;
using TourPlanner.Models;
using TourPlanner.Models.MapInformation;

namespace TourPlanner.Bll;
public interface IOpenRouteService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Task<Coordinates> GetCoordinates(string location);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<RouteInformation> GetRouteInformation(ORServiceRequestDto request);
}