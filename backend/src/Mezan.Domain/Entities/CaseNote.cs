using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class CaseNote : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string CaseId { get; set; } = string.Empty;
    public Case? Case { get; set; }

    public string Text { get; set; } = string.Empty;
}
