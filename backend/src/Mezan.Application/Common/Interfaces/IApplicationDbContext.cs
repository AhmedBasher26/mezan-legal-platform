using Mezan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Lawyer> Lawyers { get; }
    DbSet<Client> Clients { get; }
    DbSet<ImportantDate> ImportantDates { get; }
    DbSet<Case> Cases { get; }
    DbSet<Hearing> Hearings { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Template> Templates { get; }
    DbSet<CaseFile> CaseFiles { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<CaseNote> CaseNotes { get; }
    DbSet<Activity> Activities { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
