using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _handler = new DeleteTaskCommandHandler(_taskRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTask_ShouldDeleteTask()
    {
        //Arrange
        var command = new DeleteTaskCommand { Id = 1 };

        var existingTask = new TaskItem
        {
            Id = command.Id,
            Title = "Tarea a eliminar ",
            CategoryId = 1,
            IsDeleted = false,
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        _taskRepositoryMock
            .Setup(x => x.DeleteAsync(existingTask))
            .Returns((Task<TaskItem>)Task.CompletedTask);

        //Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("tarea");
        result.Error.Should().Contain(command.Id.ToString());

        _taskRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
        _taskRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseSoftDelete()
    {
        //Arrange
        var command = new DeleteTaskCommand { Id = 1};

        var existingTask = new TaskItem
        {
            Id = command.Id,
            Title = "Tarea",
            CategoryId = 1,
            IsDeleted = false,
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        //Act
        await _handler.Handle(command, CancellationToken.None);

        //Assert
        _taskRepositoryMock.Verify(x => x.DeleteAsync(existingTask), Times.Once);
    }





























}

