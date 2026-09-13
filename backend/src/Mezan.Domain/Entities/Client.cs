using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class Client : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public ICollection<ImportantDate> ImportantDates { get; set; } = new List<ImportantDate>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
