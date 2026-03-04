using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQuery : IRequest<Result<List<TaskDto>>>
{
    public int? CategoryId { get; set; }
    public bool? IsCompleted { get; set; }

}