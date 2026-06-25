namespace TourPlanner.Models;

public class User
{
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string HashedPassword { get; set; }

    public ICollection<Tour> Tours{ get; set;} = new List<Tour>();
}
