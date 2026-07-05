using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public async Task Update(string id, Tour newTour)
    {
        var tour = await _context.tours.FindAsync(id);
        if(tour is null) return;

        if(newTour.Title is not null)
            tour.Title = newTour.Title;
        if(newTour.Description is not null)
            tour.Description = newTour.Description;
        if(newTour.From is not null)
            tour.From = newTour.From;
        if(newTour.To is not null)
            tour.To = newTour.To;
        tour.TransportType = newTour.TransportType;

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
    /// <returns>Tours assigned with the UserId, and query matching Title or Description</returns>
    public async Task<IEnumerable<Tour>> ReadFromQuery(string userId, string? query)
    {
        var searchText = query?.Trim() ?? string.Empty;
        
        var tours = await _context.tours.Where(t => t.UserId == userId).ToListAsync();
        return tours.Where(t => string.IsNullOrEmpty(searchText) || FullTextSearch(t,searchText));
    }

    bool FullTextSearch(Tour tour, string text)
    {
        return
        CompareToString(tour.Title,text) ||
        CompareToString(tour.Description,text) ||
        CompareToString(tour.From,text) ||
        CompareToString(tour.To,text) ||
        CompareToString(tour.TransportType,text);
    }

    bool CompareToString<T>(T value, string text)
    {
        if(value is null) return false;
        return Convert.ToString(value)!.Contains(text);
    }
}
