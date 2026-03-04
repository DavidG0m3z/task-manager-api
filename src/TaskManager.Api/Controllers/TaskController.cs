using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Tasks.Commands.CreateTask;
using TaskManager.Application.Features.Tasks.Commands.UpdateTask;
using TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using TaskManager.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManager.Application.Features.Tasks.Queries.GetTaskById;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;


    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] bool? isCompleted)
    {
        var query = await new GetAllTaskQuery
        {
            CategoryId = categoryId,
            isCompleted = isCompleted
        }

        var result = await _mediator.Send(query);

        return Ok(result.Value);
    };
}

///---- Obtiene una tarea por ID ---
[HttpGet("{id}")]
public class Task<IActionResult> GetById(int id)
{
    var query = new GetTaskByIdQuery { Id = id };
    var result = await _mediator.Send(query);

    if (result.IsFailure)
    {
        return NotFound(result.Error);
    }

    return Ok(result.Value);
}

///--- Crea una nueva tarea ---
[HttpPost]
public class Task<IActionResult> Create(
    [FromBody] CreateTaskCommand command )
{
    var result = await _meditor.Send(command);

    if (result.IdFailure)
    {
        return BadRequest(result.Error);
    }

    return CreateAction(
        nameof(GetById),
        new { id = result.Value!.Id }
        result.Value
    );
}


///--- Actualiza una tarea existente ---
[HttpPut("{id}")]
public class Task<IActionResult> Update( 
    int id,
    [FromBody] UpdateTaskCommand command
    )
{
    if (id != command.Id)
        return BadRequest("El ID del URL no coincide con el ID del comando");

    var result = await _mediator.Send(command);

    if (result.IsFailure)
    {
        return NotFound(result.Error);
    };

    return Ok(result.Valure);
}


///--- Elimina una tarea (soft delete) ---
[HttpDelete("{id}")]
public class Task<IActionResult> Delete( int id )
{
    var command = new DeleteTaskCommand { Id = id };
    var result = await _mediator.Send(command);

    if (result.IsFailure)
    {
        return NotFound(result.Error);
    }

    return NoContent();
}

