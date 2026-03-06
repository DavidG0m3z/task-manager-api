using MediatR;
using AutoMapper;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;


namespace TaskManager.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, Result<List<TaskDto>>>
{
    private readonly ITaskRepository _taskReposity;
    private readonly IMapper _mapper;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskReposity = taskRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<TaskDto>>> Handle(
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

        var taskDto = _mapper.Map<List<TaskDto>>(tasks);

        //var taskDtos = tasks.Select(task => new TaskDto
        //{
        //    Id = task.Id,
        //    Title = task.Title,
        //    Description = task.Description,
        //    IsCompleted = task.IsCompleted,
        //    DueDate = task.DueDate,
        //    Priority = task.Priority,
        //    CategoryId = task.CategoryId,
        //    CategoryName = task.Category?.Name ?? "Sin categoría",
        //    CreatedAt = task.CreatedAt
        //}).ToList();

        return Result<List<TaskDto>>.Success(taskDto);

    }

}