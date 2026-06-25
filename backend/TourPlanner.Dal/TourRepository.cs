using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualBasic;
using TourPlanner.Models;

namespace TourPlanner.Dal;

public class TourRepository : Repository, IRepository<Tour>
{
    public TourRepository(TourPlannerDbContext tourPlannerDbContext) 
        : base(tourPlannerDbContext){}

    public async void Create(Tour tour)
    {
    _context.tours.Add(tour);
    await _context.SaveChangesAsync();
    }
    public async Task<Tour?> Read(string id) => await _context.tours.FindAsync(id);
    public async Task<IEnumerable<Tour>> ReadAll(string? userId) =>
        (IEnumerable<Tour>)_context.tours.Where(t => t.UserId == userId).ToListAsync();

    public async void Update(string id, Tour objData)
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
    public async void Delete(Tour tour)
    {
    _context.tours.Remove(tour);
    await _context.SaveChangesAsync();
    } 

}