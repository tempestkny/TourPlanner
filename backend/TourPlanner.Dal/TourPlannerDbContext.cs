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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(u => u.HashedPassword)
                .IsRequired()
                .HasMaxLength(512);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<Tour>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tours)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        modelBuilder.Entity<TourLog>()
            .HasOne(tl => tl.Tour)
            .WithMany(t => t.TourLogs)
            .HasForeignKey(tl => tl.TourId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
