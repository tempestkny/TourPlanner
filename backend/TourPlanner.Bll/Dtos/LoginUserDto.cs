using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Bll.Dtos;

public class LoginUserDto
{
    [Required]
    [MaxLength(255)]
    public required string Identifier { get; set; }

    [Required]
    [MaxLength(128)]
    public required string Password { get; set; }
}
