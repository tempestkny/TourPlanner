using System.Collections;
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
    public Tour? Read(string id) => _context.tours.Find(id);
    public IEnumerable<Tour> ReadAll(string? query = null) => _context.tours;
    public async void Update(string id, Tour objData)
    {
        var tour = await _context.tours.FindAsync(id);
        if(tour is null) return;

        await _context.SaveChangesAsync();
    }
    public async void Delete(Tour tour)
    {
    _context.tours.Remove(tour);
    await _context.SaveChangesAsync();
    } 

}