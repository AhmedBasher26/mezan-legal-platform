using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class Lawyer : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Template> Templates { get; set; } = new List<Template>();
    public ICollection<CaseFile> CaseFiles { get; set; } = new List<CaseFile>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<CaseNote> CaseNotes { get; set; } = new List<CaseNote>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
