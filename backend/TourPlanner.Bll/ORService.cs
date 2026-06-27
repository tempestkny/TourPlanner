using TourPlanner.Models;

namespace TourPlanner.Bll;

public static class ORService
{
    /// <summary>
    /// Validates if the OpenRouteService can find the given Input
    /// </summary>
    /// <param name="place"></param>
    /// <returns>true if it was found</returns>
    private static bool FindPlace(string place)
    {
        return true;
    }

    /// <summary>
    /// Gets the Time and distance of a route from two points and a travel method
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static (int, int) GetTimeAndDistance(string from, string to, TransportType transportType)
    {
        return (0,0);
    }

}