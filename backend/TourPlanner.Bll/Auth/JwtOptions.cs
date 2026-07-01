using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Bll.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    [MinLength(32)]
    public required string Secret { get; set; }

    [Range(1, 1440)]
    public int ExpirationMinutes { get; set; } = 60;
}
