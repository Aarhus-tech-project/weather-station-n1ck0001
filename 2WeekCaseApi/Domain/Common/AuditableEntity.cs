namespace Domain.Common;

public class AuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}