using FluentAssertions;
using Moq;
using TaskManager.Application.Features.Tasks.Commands.UpdateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using AutoMapper;
using Xunit;


namespace TaskManager.Tests.Features.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateTaskCommandHandler(
            _taskRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_SouldUpdateTask()
    {
        //Arrange
        var command = new UpdateTaskCommand
        {
            Id = 1,
            Title = "Tarea actualizada",
            Description = "Nueva descripción",
            CategoryId = 1,
            Priority = 3,
            DueDate = DateTime.UtcNow.AddDays(10),
            IsCompleted = true
        };

        var existingTask = new TaskItem
        {
            Id = command.Id,
            Title = "Título antiguo",
            Description = "Descripción antigua",
            CategoryId = 1,
            Priority = 1,
            IsCompleted = false
        };

        _taskRepositoryMock
           .Setup(x => x.GetByIdAsync(command.Id))
           .ReturnsAsync(existingTask);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(command.CategoryId))
            .ReturnsAsync(true);

        _taskRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TaskItem>()))
            .Returns((Task<TaskItem>)Task.CompletedTask);

        var taskDto = new Application.Common.DTOs.TaskDto
        {
            Id = command.Id,
            Title = command.Title,
            Description = command.Description
        };

        _mapperMock
            .Setup(x => x.Map<Application.Common.DTOs.TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);
    }

    [Fact]
    public async Task Handle_WithNonExistentTaks_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = 999,
            Title = "Tarea",
            Description = "Nueva descripción",
            CategoryId = 1,
            Priority = 2,
            DueDate = DateTime.UtcNow
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("tarea");
        result.Error.Should().Contain(command.Id.ToString());

        _taskRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
        _taskRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateTaskCommand
        {
            Id = 1,
            Title = "Tarea",
            Description = "Nueva descripción",
            CategoryId = 999,
            Priority = 2,
            DueDate = DateTime.UtcNow
        };

        var existingTask = new TaskItem
        {
            Id = command.Id,
            Title = "Título",
            CategoryId = 1
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(command.CategoryId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("categoría");

        _taskRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }
}

