using TourPlanner.Models;

namespace TourPlanner.Bll.Dtos;

public class ExportTourDto
{
    public required string Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public TransportType? TransportType { get; set; }
    public double Distance { get; set; }
    public double Time { get; set; }
    public ICollection<ExportTourLogDto> TourLogs { get; set; } = new List<ExportTourLogDto>();
}
