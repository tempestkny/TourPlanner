using TourPlanner.Models;

namespace TourPlanner.Dal;

public class TourRepository : Repository, IRepository<Tour>
{
    public TourRepository(string connectionString) : base(connectionString){}

    public void Create(Tour obj)
    {
        throw new NotImplementedException();
    }

    public bool Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Tour? Read(string id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Tour> ReadAll(string? query = null)
    {
        throw new NotImplementedException();
    }

    public void Update(string id, Tour objData)
    {
        throw new NotImplementedException();
    }
}