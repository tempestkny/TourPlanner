namespace TourPlanner.Models;

public class TourLog
{
    public required string Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? Comment { get; set; }

    // Navigation
    public required string TourId { get; set; }
    public required Tour Tour {get; set;}
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
