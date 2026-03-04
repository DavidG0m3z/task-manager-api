using System.Collections.Generic;
using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public required string Description { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
