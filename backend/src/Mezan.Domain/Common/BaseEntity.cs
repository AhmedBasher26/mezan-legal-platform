namespace Mezan.Domain.Common;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
}

public abstract class AuditableEntity : BaseEntity
{
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long? UpdatedAt { get; set; }
}
