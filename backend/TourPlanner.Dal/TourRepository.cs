using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Dal;

public class TourRepository : Repository, ITourRepository
{
    public TourRepository(TourPlannerDbContext tourPlannerDbContext) 
        : base(tourPlannerDbContext){}

    public async Task Create(Tour tour){
    _context.tours.Add(tour);
    await _context.SaveChangesAsync();
    }
    public async Task<Tour?> Read(string id) => await _context.tours.FindAsync(id);

    public async Task Update(string id, Tour objData)
    {
        var tour = await _context.tours.FindAsync(id);
        if(tour is null) return;

        tour.Title = objData.Title;
        tour.TourDescription = objData.TourDescription;
        tour.TransportType = objData.TransportType;
        tour.From = objData.From;
        tour.To = objData.To;

        await _context.SaveChangesAsync();
    }
    public async Task Delete(Tour tour)
    {
    _context.tours.Remove(tour);
    await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Returns a List of Tours dependend on a Search query
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<Tour>> ReadFromQuery(string userId, string? query)
    {
        return await _context.tours.Where(t => t.UserId == userId).ToListAsync();

    }
}