using MediatR;
using AutoMapper;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return Result<TaskDto>.Failure($"La categoria con ID {request.CategoryId}, no existe");
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Priority = request.Priority,
            DueDate = request.DueDate,
            IsCompleted = false
        };

        var savedTask = await _taskRepository.SaveAsync(task);

        var taskDto = _mapper.Map<TaskDto>(savedTask);

        //var taskDto = new TaskDto
        //{
        //    Id = savedTask.Id,
        //    Title = savedTask.Title,
        //    Description = savedTask.Description,
        //    IsCompleted = savedTask.IsCompleted,
        //    DueDate = savedTask.DueDate,
        //    Priority = savedTask.Priority,
        //    CategoryId = savedTask.CategoryId,
        //    CategoryName = category!.Name,
        //    CreatedAt = savedTask.CreatedAt
        //};

        return Result<TaskDto>.Success(taskDto);
    }

}