namespace TourPlanner.Models;

public class User
{
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? HashedPassword { get; set; }

    public ICollection<Tour> Tours{ get; set;} = new List<Tour>();
}
