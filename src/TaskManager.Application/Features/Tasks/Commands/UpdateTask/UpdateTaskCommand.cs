using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest<Result<TaskDto>>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public int CategoryId { get; set; }
}