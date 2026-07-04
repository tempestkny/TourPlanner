using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Bll.Dtos;

public class ImportTourDataDto
{
    [Required]
    public ICollection<ImportTourDto> Tours { get; set; } = new List<ImportTourDto>();
}
