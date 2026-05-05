namespace TourPlanner.Models;

public class TourLog
{
    public required string id { get; set; }
    public DateTime timeStamp { get; set; }
    public string? comment { get; set; }
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
