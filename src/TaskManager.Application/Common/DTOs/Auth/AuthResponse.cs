using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Common.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }
    }
}
