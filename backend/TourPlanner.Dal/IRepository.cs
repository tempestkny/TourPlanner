namespace TourPlanner.Dal;

public interface IRepository<T>
{
    Task<IEnumerable<T>> ReadAll(string? query = null);
    Task<T?> Read(string id);
    void Create(T obj);
    void Update(string id,T objData);
    void Delete(T obj);
}
