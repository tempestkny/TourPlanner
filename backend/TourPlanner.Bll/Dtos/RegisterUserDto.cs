using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Bll.Dtos;

public class RegisterUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public required string Username { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public required string Password { get; set; }
}