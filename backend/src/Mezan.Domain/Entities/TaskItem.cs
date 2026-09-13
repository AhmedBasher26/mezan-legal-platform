using Mezan.Domain.Common;
using Mezan.Domain.Enums;

namespace Mezan.Domain.Entities;

public class TaskItem : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string? Time { get; set; } // HH:mm
    public Priority Priority { get; set; } = Priority.Medium;

    public string? CaseId { get; set; }
    public Case? Case { get; set; }

    public string? ClientId { get; set; }
    public Client? Client { get; set; }

    public bool Completed { get; set; }
    public long? CompletedAt { get; set; }
}
