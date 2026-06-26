namespace TourPlanner.Models;

public class TourLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime TimeStamp { get; set; }
    public string? Comment { get; set; }

    // Navigation
    public string? TourId { get; set; }
    public required Tour Tour {get; set;}
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
