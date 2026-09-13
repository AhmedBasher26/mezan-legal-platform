using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class Hearing : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string CaseId { get; set; } = string.Empty;
    public Case? Case { get; set; }

    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string Time { get; set; } = string.Empty; // HH:mm
    public string Type { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
