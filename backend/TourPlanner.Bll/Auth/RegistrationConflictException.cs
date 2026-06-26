namespace TourPlanner.Bll.Auth;

public class RegistrationConflictException : Exception
{
    public RegistrationConflictException(string field)
        : base($"A user with this {field} already exists.")
    {
        Field = field;
    }

    public string Field { get; }
}
