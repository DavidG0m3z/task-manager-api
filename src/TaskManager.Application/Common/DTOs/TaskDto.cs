namespace TaskManager.Application.Common.DTOs;


public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
}