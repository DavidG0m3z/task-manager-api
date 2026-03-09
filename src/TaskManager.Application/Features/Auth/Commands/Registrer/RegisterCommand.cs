using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common.DTOs.Auth;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Auth.Commands.Registrer
{
    public class RegisterCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
