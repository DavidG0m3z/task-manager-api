using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands.Login;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Xunit;

namespace TaskManager.Tests.Features.Auth
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly LoginCommandHandler _handler;
        public LoginCommandHandlerTests()
        {

            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _handler = new LoginCommandHandler(_userRepositoryMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidationCredentials_ShouldReturnToken()
        {
            //Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

            var user = new User
            {
                Id = 1,
                Email = command.Email,
                PasswordHash = passwordHash,
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Role = new Role { Id = 2, Name = "User" }
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _jwtServiceMock
                .Setup(x => x.GenerateToken(user))
                .Returns("fake-jwt-token");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Token.Should().Be("fake-jwt-token");
            result.Value.Email.Should().Be(command.Email);
            result.Value.Role.Should().Be("User");
            result.Value.FullName.Should().Be("Test User");

            _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
            _jwtServiceMock.Verify(x => x.GenerateToken(user), Times.Once);

        }

        [Fact]
        public async Task Handle_WithNonExistetEmail_ShouldReturnFailure()
        {
            //Arrange
            var command = new LoginCommand
            {
                Email = "nonexistent@example.com",
                Password = "Test123!"
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Credenciales invalidas");

            _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidPassword_ShouldReturnFailure()
        {
            //Arrage
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            var correctPasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");

            var user = new User
            {
                Id = 1,
                Email = command.Email,
                PasswordHash = correctPasswordHash,
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Role = new Role { Id = 2, Name = "User" }
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Password invalido");

            _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInactiveUser_ShouldReturnFailure()
        {
            //Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
            var user = new User
            {
                Id = 1,
                Email = command.Email,
                PasswordHash = passwordHash,
                FirstName = "Test",
                LastName = "User",
                IsActive = false,
                Role = new Role { Id = 2, Name = "User" }
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Usuario inactivo");

            _userRepositoryMock.Verify(x => x.GetByEmailAsync(command.Email), Times.Once);
            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Handle_WithEmptyPassword_ShouldReturnFailure(string password)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = password
            };

            var user = new User
            {
                Id = 1,
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Role = new Role { Id = 2, Name = "User" }
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

    }

}
