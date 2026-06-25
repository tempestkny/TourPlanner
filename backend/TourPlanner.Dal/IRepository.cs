namespace TourPlanner.Dal;

public interface IRepository<T>
{
    IEnumerable<T> ReadAll(string? query = null);
    T? Read(string id);
    void Create(T obj);
    void Update(string id,T objData);
    void Delete(T obj);
}
