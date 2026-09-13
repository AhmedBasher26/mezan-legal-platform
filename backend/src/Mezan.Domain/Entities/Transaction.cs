using Mezan.Domain.Common;
using Mezan.Domain.Enums;

namespace Mezan.Domain.Entities;

public class Transaction : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string ClientId { get; set; } = string.Empty;
    public Client? Client { get; set; }

    public string? CaseId { get; set; }
    public Case? Case { get; set; }

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Fees;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
    public string? Notes { get; set; }
}
