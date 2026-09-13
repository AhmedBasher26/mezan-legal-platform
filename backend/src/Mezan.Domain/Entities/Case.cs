using Mezan.Domain.Common;
using Mezan.Domain.Enums;

namespace Mezan.Domain.Entities;

public class Case : AuditableEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string ClientId { get; set; } = string.Empty;
    public Client? Client { get; set; }

    public string CaseNumber { get; set; } = string.Empty;
    public string Court { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string FiledDate { get; set; } = string.Empty; // YYYY-MM-DD
    public string? NextHearingDate { get; set; }
    public string? NextHearingTime { get; set; }
    public CaseStatus Status { get; set; } = CaseStatus.Open;
    public string? Description { get; set; }
    public string? Notes { get; set; }

    public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    public ICollection<CaseFile> CaseFiles { get; set; } = new List<CaseFile>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<CaseNote> NotesList { get; set; } = new List<CaseNote>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
