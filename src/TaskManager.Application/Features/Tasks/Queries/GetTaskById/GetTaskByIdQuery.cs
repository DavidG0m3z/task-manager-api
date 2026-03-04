using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQuery : IRequest<Result<TaskDto>>
{
    public int Id { get; set; }
}