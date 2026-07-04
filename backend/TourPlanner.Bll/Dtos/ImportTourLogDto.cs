using System.ComponentModel.DataAnnotations;
using TourPlanner.Models;

namespace TourPlanner.Bll.Dtos;

public class ImportTourLogDto
{
    [Required]
    public DateTime TimeStamp { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    [Required]
    public Difficulty Difficulty { get; set; }

    [Range(0, double.MaxValue)]
    public double TotalDistance { get; set; }

    [Range(0, double.MaxValue)]
    public double TotalTime { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }
}
