using Npgsql;

namespace TourPlanner.Dal;

public class Repository
{
    protected NpgsqlConnection? connnectionString;
    public Repository(string connectionString)
    {
        connnectionString = new NpgsqlConnection(connectionString);
        connnectionString.Open();
    }
}
