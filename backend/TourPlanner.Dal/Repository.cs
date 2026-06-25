namespace TourPlanner.Dal;

public class Repository
{
    protected readonly TourPlannerDbContext _context;
    public Repository(TourPlannerDbContext context)
    {
        _context = context;
    }
}
