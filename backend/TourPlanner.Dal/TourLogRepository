using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Dal;

public class TourLogRepository : Repository, ITourLogRepository
{
    public TourLogRepository(TourPlannerDbContext context) : base(context)
    {
    }

    public async Task Create(TourLog tourLog)
    {
        _context.tourLogs.Add(tourLog);
        await _context.SaveChangesAsync();
    }

    public async Task<TourLog?> Read(string id) =>
        await _context.tourLogs.FindAsync(id);

    public async Task<IEnumerable<TourLog>> ReadByTourId(string tourId) =>
        await _context.tourLogs
            .Where(tourLog => tourLog.TourId == tourId)
            .OrderByDescending(tourLog => tourLog.TimeStamp)
            .ToListAsync();

    public async Task Update(string id, TourLog objData)
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

    public async Task Delete(TourLog tourLog)
    {
        _context.tourLogs.Remove(tourLog);
        await _context.SaveChangesAsync();
    }
}
