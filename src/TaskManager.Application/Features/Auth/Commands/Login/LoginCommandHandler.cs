using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common.DTOs.Auth;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler( 
            IUserRepository userRepository, 
            IJwtService jwtService )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync( request.Email );

            if ( user == null )
            {
                return Result<AuthResponse>.Failure("Credenciales invalidas");
            }

            if ( !user.IsActive)
            {
                return Result<AuthResponse>.Failure("Usuario inactivo");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if ( !isPasswordValid )
            {
                return Result<AuthResponse>.Failure("Password invalido");
            }

            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.Name,
                ExpiresIn = 3600
            };

            return Result<AuthResponse>.Success(response);

        }

    }
}
