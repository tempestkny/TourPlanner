namespace TourPlanner.Tests;

using System.Data;
using Npgsql;

public class PostgresSetupTest
{
    string connectionString ="Host=localhost;Port=5432;Username=admin;Password=SWENSS26;Database=tourplannerdb";
    private NpgsqlConnection connection;

    [SetUp]
    public void Setup()
    {
        connection = new NpgsqlConnection(connectionString);
    }

    [TearDown]
    public void TearDown()
    {
        connection?.Dispose();
    }

    [Test]
    public void TestIfServerIsOnlineAndReachable()
    {
        try
        {
            connection.Open();
            Console.WriteLine("Connected to PostgreSQL!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
        connection.Close();
    }

}
