using Mezan.Domain.Common;
using Mezan.Domain.Enums;

namespace Mezan.Domain.Entities;

public class Activity : BaseEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public ActivityKind Kind { get; set; } = ActivityKind.Auth;
    public string Text { get; set; } = string.Empty;
    public string? CaseId { get; set; }
    public Case? Case { get; set; }
    public long At { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
