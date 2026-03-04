using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}