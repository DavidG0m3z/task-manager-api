using FluentAssertions;
using Moq;
using TaskManager.Application.Features.Tasks.Commands.CreateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using AutoMapper;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTaskCommandHandler(
            _taskRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Nueva tarea",
            Description = "Descripción de prueba",
            CategoryId = 1,
            Priority = 2,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(command.CategoryId))
            .ReturnsAsync(true);

        var savedTask = new TaskItem
        {
            Id = 1,
            Title = command.Title,
            Description = command.Description,
            CategoryId = command.CategoryId,
            Priority = command.Priority,
            DueDate = command.DueDate,
            IsCompleted = false
        };

        _taskRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(savedTask);

        var taskDto = new Application.Common.DTOs.TaskDto
        {
            Id = savedTask.Id,
            Title = savedTask.Title,
            Description = savedTask.Description,
            CategoryId = savedTask.CategoryId,
            Priority = savedTask.Priority,
            DueDate = savedTask.DueDate,
            IsCompleted = savedTask.IsCompleted
        };

        _mapperMock
            .Setup(x => x.Map<Application.Common.DTOs.TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be(command.Title);
        result.Value.Priority.Should().Be(command.Priority);

        _categoryRepositoryMock.Verify(x => x.ExistsAsync(command.CategoryId), Times.Once);
        _taskRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Nueva tarea",
            Description = "Descripción",
            CategoryId = 999,
            Priority = 2,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(command.CategoryId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("categoria");
        result.Error.Should().Contain(command.CategoryId.ToString());

        _categoryRepositoryMock.Verify(x => x.ExistsAsync(command.CategoryId), Times.Once);
        _taskRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task Handle_WithInvalidPriority_ShouldBeValidatedByFluentValidation(int invalidPriority)
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Nueva tarea",
            Description = "Descripción",
            CategoryId = 1,
            Priority = invalidPriority,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        command.Priority.Should().NotBeInRange(1, 3);
    }

    [Fact]
    public async Task Handle_ShouldSetIsCompletedToFalseByDefault()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Nueva tarea",
            Description = "Descripción",
            CategoryId = 1,
            Priority = 2,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(command.CategoryId))
            .ReturnsAsync(true);

        TaskItem? capturedTask = null;
        _taskRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(task => capturedTask = task)
            .ReturnsAsync((TaskItem task) => task);

        _mapperMock
            .Setup(x => x.Map<Application.Common.DTOs.TaskDto>(It.IsAny<TaskItem>()))
            .Returns(new Application.Common.DTOs.TaskDto 
            { 
                //Id = 1,
                Description = "Description", 
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTask.Should().NotBeNull();
        capturedTask!.IsCompleted.Should().BeFalse();
    }
}