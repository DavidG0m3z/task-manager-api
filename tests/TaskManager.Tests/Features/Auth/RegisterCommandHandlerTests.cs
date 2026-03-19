using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using FluentAssertions;
using TaskManager.Application.Features.Auth.Commands.Registrer;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Xunit;

namespace TaskManager.Tests.Features.Auth
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();

            _handler = new RegisterCommandHandler(

                _userRepositoryMock.Object,
                _jwtServiceMock.Object

            );
        }

        [Fact]
        public async Task Handle_WhenEmailDoesNoExist_ShouldCreateUserAndReturnToken()
        {

            // ARRANGE (Preparar)

            var command = new RegisterCommand
            {
                Email = "test@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User",
            };

            _userRepositoryMock
                .Setup(x => x.EmailExistsAsync(command.Email))
                .ReturnsAsync(false);


            var savedUser = new User
            {
                Id = 1,
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                RoleId = 2,
                Role = new Role { Id = 2, Name = "User" }
            };

            _userRepositoryMock
                .Setup( x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(savedUser);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(savedUser.Id))
                .ReturnsAsync(savedUser);

            _jwtServiceMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("fake-jwt-token-12345");



            // ACT (Actuar)

            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT (Verificar)

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value!.Token.Should().Be("fake-jwt-token-12345");
            result.Value.Email.Should().Be(command.Email);
            result.Value.FullName.Should().Be("Test User");
            result.Value.Role.Should().Be("User");
            result.Value.ExpiresIn.Should().Be(3600);

            _userRepositoryMock.Verify(
                x => x.EmailExistsAsync(command.Email),
                Times.Once
             );

            _userRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<User>()),
                Times.Once
             );

            _userRepositoryMock.Verify(
                x => x.GetByIdAsync(savedUser.Id),
                Times.Once
            );

            _jwtServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Once
             );
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ShouldReturnFailure()
        {
            // ARRANGE (Preparar)

            var command = new RegisterCommand
            {
                Email = "existing@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock
                .Setup(x => x.EmailExistsAsync(command.Email))
                .ReturnsAsync(true);

            // ACT (Actuar)
            
            var result = await _handler.Handle(command, CancellationToken.None);


            // ASSERT (Verificar)

            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be("El email ya está registrado");

            _userRepositoryMock.Verify(
                x => x.EmailExistsAsync(command.Email),
                Times.Once
             );

            _userRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<User>()),
                Times.Never
             );

            _jwtServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Never
             );
        }

        [Fact]
        public async Task Handle_WhenPasswordIsHashed_ShouldNotStorePasswordInPlainText()
        {
            // ARRANGE (Preparar)

            var command = new RegisterCommand
            {
                Email = "secure@example.com",
                Password = "MyPassword123!",
                FirstName = "Secure",
                LastName = "User"
            };

            _userRepositoryMock
                .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            User? capturedUser = null;

            _userRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => capturedUser = user)
                .ReturnsAsync((User user) => user);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Email = command.Email,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    RoleId = 2,
                    Role = new Role { Id = 2, Name = "User" }
                });

            _jwtServiceMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("token");

            // ACT (Actuar)

            await _handler.Handle(command, CancellationToken.None);

            // ASSERT (Verificar)

            capturedUser.Should().NotBeNull();

            capturedUser!.PasswordHash.Should().NotBeNull();

            capturedUser.PasswordHash.Should().StartWith("$2a$");

            capturedUser.PasswordHash.Length.Should().BeGreaterThan(50);

        }
    }
}
