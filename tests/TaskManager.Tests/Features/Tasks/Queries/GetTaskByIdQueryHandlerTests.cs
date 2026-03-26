using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using TaskManager.Application.Features.Tasks.Queries.GetTaskById;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using AutoMapper;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Queries;
public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTaskByIdHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTaskByIdHandler(
            _taskRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingTask_ShouldReturnTask()
    {
        // Arrange
        var query = new GetTaskByIdQuery { Id = 1 };

        var task = new TaskItem
        {
            Id = 1,
            Title = "Tarea de prueba",
            Description = "Descripción",
            CategoryId = 1,
            Priority = 2,
            IsCompleted = false
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(query.Id))
            .ReturnsAsync(task);

        var taskDto = new Application.Common.DTOs.TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description
        };

        _mapperMock
            .Setup(x => x.Map<Application.Common.DTOs.TaskDto>(task))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(task.Id);
        result.Value.Title.Should().Be(task.Title);

        _taskRepositoryMock.Verify(x => x.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTask_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetTaskByIdQuery { Id = 999 };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(query.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("tarea");
        result.Error.Should().Contain(query.Id.ToString());

        _taskRepositoryMock.Verify(x => x.GetByIdAsync(query.Id), Times.Once);
        _mapperMock.Verify(x => x.Map<Application.Common.DTOs.TaskDto>(It.IsAny<TaskItem>()), Times.Never);
    }
}

