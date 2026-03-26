using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using TaskManager.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using AutoMapper;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Queries;
public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTasksQueryHandler _handler;

    public GetAllTasksQueryHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTasksQueryHandler(
            _taskRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnAllTasks()
    {
        // Arrange
        var query = new GetAllTasksQuery();

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Tarea 1", CategoryId = 1, IsCompleted = false },
            new TaskItem { Id = 2, Title = "Tarea 2", CategoryId = 2, IsCompleted = true },
            new TaskItem { Id = 3, Title = "Tarea 3", CategoryId = 1, IsCompleted = false }
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        var taskDtos = tasks.Select(t => new Application.Common.DTOs.TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = "Nueva descripcion"
        }).ToList();

        _mapperMock
            .Setup(x => x.Map<List<Application.Common.DTOs.TaskDto>>(It.IsAny<List<TaskItem>>()))
            .Returns(taskDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        _taskRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCategoryIdFilter_ShouldReturnFilteredTasks()
    {
        // Arrange
        var query = new GetAllTasksQuery { CategoryId = 1 };

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Tarea 1", CategoryId = 1, IsCompleted = false },
            new TaskItem { Id = 2, Title = "Tarea 2", CategoryId = 2, IsCompleted = true },
            new TaskItem { Id = 3, Title = "Tarea 3", CategoryId = 1, IsCompleted = false }
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        _mapperMock
            .Setup(x => x.Map<List<Application.Common.DTOs.TaskDto>>(It.IsAny<List<TaskItem>>()))
            .Returns((List<TaskItem> source) => source.Select(t => new Application.Common.DTOs.TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = "Nueva descripcion",
                CategoryId = t.CategoryId
            }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(t => t.CategoryId.Should().Be(1));
    }

    [Fact]
    public async Task Handle_WithIsCompletedFilter_ShouldReturnFilteredTasks()
    {
        // Arrange
        var query = new GetAllTasksQuery { IsCompleted = true };

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Tarea 1", CategoryId = 1, IsCompleted = false },
            new TaskItem { Id = 2, Title = "Tarea 2", CategoryId = 2, IsCompleted = true },
            new TaskItem { Id = 3, Title = "Tarea 3", CategoryId = 1, IsCompleted = true }
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        _mapperMock
            .Setup(x => x.Map<List<Application.Common.DTOs.TaskDto>>(It.IsAny<List<TaskItem>>()))
            .Returns((List<TaskItem> source) => source.Select(t => new Application.Common.DTOs.TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = "Nueva descripcion",
                IsCompleted = t.IsCompleted
            }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(t => t.IsCompleted.Should().BeTrue());
    }

    [Fact]
    public async Task Handle_WithBothFilters_ShouldReturnFilteredTasks()
    {
        // Arrange
        var query = new GetAllTasksQuery
        {
            CategoryId = 1,
            IsCompleted = false
        };

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Tarea 1", CategoryId = 1, IsCompleted = false },
            new TaskItem { Id = 2, Title = "Tarea 2", CategoryId = 2, IsCompleted = true },
            new TaskItem { Id = 3, Title = "Tarea 3", CategoryId = 1, IsCompleted = true },
            new TaskItem { Id = 4, Title = "Tarea 4", CategoryId = 1, IsCompleted = false }
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        _mapperMock
            .Setup(x => x.Map<List<Application.Common.DTOs.TaskDto>>(It.IsAny<List<TaskItem>>()))
            .Returns((List<TaskItem> source) => source.Select(t => new Application.Common.DTOs.TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = "Nueva descripcion",
                CategoryId = t.CategoryId,
                IsCompleted = t.IsCompleted
            }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(t =>
        {
            t.CategoryId.Should().Be(1);
            t.IsCompleted.Should().BeFalse();
        });
    }

    [Fact]
    public async Task Handle_WithNoMatchingTasks_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllTasksQuery { CategoryId = 999 };

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Tarea 1", CategoryId = 1 },
            new TaskItem { Id = 2, Title = "Tarea 2", CategoryId = 2 }
        };

        _taskRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        _mapperMock
            .Setup(x => x.Map<List<Application.Common.DTOs.TaskDto>>(It.IsAny<List<TaskItem>>()))
            .Returns(new List<Application.Common.DTOs.TaskDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
