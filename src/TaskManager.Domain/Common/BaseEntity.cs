namespace TaskManager.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeleteAt { get; set; }
}