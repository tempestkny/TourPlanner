using Microsoft.AspNetCore.Mvc.Testing;


namespace TourPlanner.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{

}

[TestFixture]
public class TourControllerAuthTest
{
    private ApiFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new ApiFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _factory?.Dispose();
        _client?.Dispose();
    }

    [Test]
    public async Task TestAllUnAuthorized_ShouldReturnUnAuthorized()
    {
        
    }

    [Test]
    public async Task TestGetAllAuthorized_ShouldReturnOk()
    {
       
    }

    [Test]
    public async Task TestCreateTourAuthorized_ShouldReturnCreated()
    {
      
    }
}
