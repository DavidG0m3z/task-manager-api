using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}