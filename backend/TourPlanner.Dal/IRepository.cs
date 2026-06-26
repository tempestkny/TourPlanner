namespace TourPlanner.Dal;

public interface IRepository<T>
{
    Task<T?> Read(string id);
    Task Create(T obj);
    Task Update(string id, T objData);
    Task Delete(T obj);
}
