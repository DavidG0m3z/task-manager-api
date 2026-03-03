using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<List<TaskItem>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetCompletedTaskAsync()
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => t.IsCompleted)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetPendingTaskAsync()
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => !t.IsCompleted)
            .ToListAsync();
    }
}
