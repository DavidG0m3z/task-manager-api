using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<bool>>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<bool>> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task == null)
        {
            return Result<bool>.Failure($"La tarea con ID {request.Id} no existe");
        }


        await _taskRepository.DeleteAsync(task);

        return Result<bool>.Success(true);

    }
}
