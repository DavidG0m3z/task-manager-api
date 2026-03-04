using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 1; 

    //Foreing Key
    public int CategoryId { get; set; }

    //Navegacion
    public Category Category { get; set; } = null!;
}