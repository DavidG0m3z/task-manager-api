using TaskManager.Domain.Common;

namespace TaskManager.Domain.Interfaces;

public interface IRepository<T> where T: BaseEntity
{
    Task<T?> GetByIdAsync(int Id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
}
