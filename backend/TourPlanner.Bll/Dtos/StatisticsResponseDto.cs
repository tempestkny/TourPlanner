namespace TourPlanner.Bll.Dtos;

public class StatisticsResponseDto
{
    public int TotalTours { get; set; }
    public int TotalTourLogs { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public double AverageRating { get; set; }
}
