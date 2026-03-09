using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.DTOs.Auth.Commands.Login;
using TaskManager.Application.Features.Auth.Commands.Login;
using TaskManager.Application.Features.Auth.Commands.Register;
using TaskManager.Application.Features.Auth.Commands.Registrer;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsFailure)
            {
                return Unauthorized(new { message = result.Error });

            }

            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody], RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsFailure)
            {
                return BadRequest(new {message = result.Error});
            }

            return CreatedAtAction(nameof(Login), result.Value);
        } 

    }
}
