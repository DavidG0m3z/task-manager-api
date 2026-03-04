using MediatR;
using TaskManager.Application.Common.DTOs;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommand : IRequest<Result<TaskDto>>
{
	public string Title { get; set; } = string.Empty;
    public required string Description { get; set; }
	public int CategoryId { get; set; }
	public int Priority { get; set; } = 1;
	public DateTime? DueDate { get; set; }
}