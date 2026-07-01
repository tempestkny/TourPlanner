using TourPlanner.Models;

namespace TourPlanner.Bll;
public interface IOpenRouteService
{
    /// <summary>
    /// Returns a Tuple with the route distance in meters and route duration in minutes
    /// </summary>
    /// <param name="start"> name of the starting location</param>
    /// <param name="dest"> name of the destination</param>
    /// <param name="profile"> method of travel</param>
    /// <returns>(distanceInMeters [double], timeInMinutes [double])</returns>
    public Task<(double distM, double timeMin)> GetTimeAndDistance(string start, string dest, TransportType profile);
}