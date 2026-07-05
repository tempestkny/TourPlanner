namespace TourPlanner.Bll.Dtos;

public class ExportTourDataDto
{
    public DateTime ExportedAt { get; set; }
    public ICollection<ExportTourDto> Tours { get; set; } = new List<ExportTourDto>();
}
