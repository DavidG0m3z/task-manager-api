using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common.DTOs.Auth;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Features.Auth.Commands.Registrer
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> Handle( 
            RegisterCommand request, 
            CancellationToken cancellationToken )
        {
            bool emailExist = await _userRepository.EmailExistsAsync(request.Email);
            
            if (emailExist)
            {
                return Result<AuthResponse>.Failure("El email ya está registrado");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                RoleId = 2  
            };

            var saveUser = await _userRepository.AddAsync(user);

            var userWithRole = await _userRepository.GetByIdAsync(saveUser.Id);

            if (userWithRole == null)
            {
                return Result<AuthResponse>.Failure("Error al crear el usuario");
            }

            var token = _jwtService.GenerateToken(userWithRole);

            var response = new AuthResponse
            {
                Token = token,
                Email = userWithRole.Email,
                FullName = userWithRole.FullName,
                Role = userWithRole.Role.Name,
                ExpiresIn = 3600
            };

            return Result<AuthResponse>.Success(response);


        }
    }
}
