using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IResquestHandler<GetAllTasksQuery, Result<List<TaskDto>>>
{
    private readonly ITaskRepository _taskReposity;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskReposity = taskRepository;
    }

    public async Task<Result<List<TaskDto>>> Handler(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskReposity.GetAllAsync();

        if (request.CategoryId.HasValue)
        {
            tasks = tasks
                .Where(t => t.CategoryId == request.CategoryId.Value)
                .ToList();
        }

        if (request.IsCompleted.HasValue)
        {
            tasks = tasks
                .Where(t => t.IsCompleted == request.IsCompleted.Value)
                .ToList();
        }

        var taskDtos = tasks.Select(task => new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate,
            Priority = task.Priority,
            CategoryId = task.CategoryId,
            CategoryName = task.Category?.Name ?? "Sin categoría",
            CreatedAt = task.CreatedAt
        }).ToList();

        return Result<List<TaskDto>>.Success(taskDtos);

    }

}