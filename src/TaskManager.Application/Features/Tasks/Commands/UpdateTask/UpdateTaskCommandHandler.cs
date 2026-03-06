using MediatR;
using AutoMapper; 
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _TaskRepository;
    private readonly ICategoryRepository _CategoryRepository;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _TaskRepository = taskRepository;
        _CategoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<TaskDto>> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken
        )
    {
        var task = await _TaskRepository.GetByIdAsync(request.Id);

        if (task == null)
        {
            return Result<TaskDto>.Failure($"La tarea con el ide {request.Id} no exciste");
        }

        var categoryExists = await _CategoryRepository.ExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return Result<TaskDto>.Failure($"La categoría con ID {request.CategoryId} no existe");
        }

        var category = await _CategoryRepository.GetByIdAsync(request.CategoryId);

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.CategoryId = request.CategoryId;

        await _TaskRepository.UpdateAsync(task);

        var taskDto = _mapper.Map<TaskDto>(task);

        //var taskDto = new TaskDto
        //{
        //    Id = task.Id,
        //    Title = task.Title,
        //    Description = task.Description,
        //    IsCompleted = task.IsCompleted,
        //    DueDate = task.DueDate,
        //    Priority = task.Priority,
        //    CategoryId = task.CategoryId,
        //    CategoryName = category!.Name,
        //    CreatedAt = task.CreatedAt
        //};

        return Result<TaskDto>.Success(taskDto);


    }
}
