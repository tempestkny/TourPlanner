namespace TourPlanner.Models;

public class User
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string HashedPassword { get; set; }
}
