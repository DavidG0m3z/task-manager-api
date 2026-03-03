using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdHandler : IRequestHandler<GetTaksByIdQuery, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Taks<Result<TaskDto>> handler( 
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id);

        if (task == null)
        {
            return Result<TaskDto>.Failure($"La tarea con id {request.Id} no fue enconntrada");
        }


        var taskDto = new TaskDto
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
        };

        return Result<TaskDto>.Success(taskDto);
    }
}