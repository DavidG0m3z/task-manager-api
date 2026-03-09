using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Common.DTOs.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
