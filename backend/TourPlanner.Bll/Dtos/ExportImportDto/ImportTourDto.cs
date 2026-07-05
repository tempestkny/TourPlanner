using System.ComponentModel.DataAnnotations;
using TourPlanner.Models;

namespace TourPlanner.Bll.Dtos;

public class ImportTourDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public required string From { get; set; }

    [Required]
    [MaxLength(255)]
    public required string To { get; set; }

    [Required]
    public TransportType TransportType { get; set; }

    [Required]
    public required string RouteInfo{get; set;}

    public ICollection<ImportTourLogDto> TourLogs { get; set; } = new List<ImportTourLogDto>();
}
