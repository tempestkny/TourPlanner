using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Dal;

public class TourLogRepository : Repository, ITourLogRepository
{
    public TourLogRepository(TourPlannerDbContext context) : base(context)
    {
    }

    public async Task Create(TourLog tourLog) // saves a new tour log in the database
    {
        _context.tourLogs.Add(tourLog);
        await _context.SaveChangesAsync();
    }

    public async Task<TourLog?> Read(string id) => // finds log by primary key (id)
        await _context.tourLogs.FindAsync(id);

    public async Task<IEnumerable<TourLog>> ReadByTourId(string tourId) =>  // finds all logs, sorts them by timestamp, returns as list
        await _context.tourLogs
            .Where(tourLog => tourLog.TourId == tourId)
            .OrderByDescending(tourLog => tourLog.TimeStamp)
            .ToListAsync();

    public async Task Update(string id, TourLog objData) // loads log by id, updates, saves changes in db
    {
        var tourLog = await _context.tourLogs.FindAsync(id);
        if (tourLog is null) return;

        tourLog.TimeStamp = objData.TimeStamp;
        tourLog.Comment = objData.Comment;
        tourLog.Difficulty = objData.Difficulty;
        tourLog.TotalDistance = objData.TotalDistance;
        tourLog.TotalTime = objData.TotalTime;
        tourLog.Rating = objData.Rating;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(TourLog tourLog) // removes a tour log from the db
    {
        _context.tourLogs.Remove(tourLog);
        await _context.SaveChangesAsync();
    }
}
