using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Domain.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<List<TaskItem>> GetByCategoryIdAsync(int categoryId);
    Task<List<TaskItem>> GetCompletedTaskAsync();
    Task<List<TaskItem>> GetPendingTaskAsync();
}