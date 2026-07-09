namespace TourPlanner.Models;

public class TourLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime TimeStamp { get; set; }
    public string? Comment { get; set; }
    public Difficulty Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public int Rating { get; set; }

    // Navigation
    public string? TourId { get; set; } // Foreign key for the Tour entity
    public required Tour Tour {get; set;} // Navigation property
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
