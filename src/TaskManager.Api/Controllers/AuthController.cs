using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Auth.Commands.Login;
//using TaskManager.Application.Features.Auth.Commands.Register;
using TaskManager.Application.Features.Auth.Commands.Registrer;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("=== LOGIN ENDPOINT LLAMADO ===");
        _logger.LogInformation("Email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        _logger.LogInformation("Result IsSuccess: {IsSuccess}", result.IsSuccess);
        _logger.LogInformation("Result IsFailure: {IsFailure}", result.IsFailure);

        if (result.IsFailure)
        {
            _logger.LogWarning("Login falló: {Error}", result.Error);
            return Unauthorized(new { message = result.Error });
        }

        _logger.LogInformation("Login exitoso, retornando token");
        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }
}