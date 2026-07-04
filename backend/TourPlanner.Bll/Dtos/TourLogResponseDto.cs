using TourPlanner.Models;

namespace TourPlanner.Bll.Dtos;

public class TourLogResponseDto
{
    public required string Id { get; set; }
    public required string TourId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? Comment { get; set; }
    public Difficulty Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public int Rating { get; set; }
}
