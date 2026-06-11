namespace TourPlanner.Dal;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

// https://learn.microsoft.com/de-de/ef/ef6/fundamentals/working-with-dbcontext
public class TourPlannerDbContext : DbContext
{
    // DbSets represent tables
    public DbSet<Tour> tours {get; set;}
    public DbSet<TourLog> tourLogs {get; set;}
    public DbSet<User> users {get; set;}

    // Constructor
    public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Custom Config


    }

}
