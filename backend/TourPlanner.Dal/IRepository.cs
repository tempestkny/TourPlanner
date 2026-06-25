namespace TourPlanner.Dal;

public interface IRepository<T>
{
    Task<T?> Read(string id);
    void Create(T obj);
    void Update(string id,T objData);
    void Delete(T obj);
}
