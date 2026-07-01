namespace TourPlanner.Bll.Auth;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid username/email or password.")
    {}
}
