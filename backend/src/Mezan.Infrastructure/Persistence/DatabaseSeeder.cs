using Mezan.Application.Common;
using Mezan.Application.Common.Interfaces;
using Mezan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Apply pending migrations or ensure database is created
        await _context.Database.EnsureCreatedAsync(cancellationToken);

        const string demoEmail = "demo@mezan.sa";
        var demoLawyer = await _context.Lawyers.FirstOrDefaultAsync(l => l.Email == demoEmail, cancellationToken);

        if (demoLawyer == null)
        {
            demoLawyer = new Lawyer
            {
                Id = "lawyer-demo",
                Name = "أ. عبدالله الحربي",
                Email = demoEmail,
                Phone = "0500000000",
                PasswordHash = _passwordHasher.HashPassword("123456"),
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 120 * 86400_000L
            };

            _context.Lawyers.Add(demoLawyer);

            var (clients, cases, hearings, tasks, templates, caseFiles, txs, notes, activities) =
                SeedDataBuilder.BuildDemoData(demoLawyer.Id);

            _context.Clients.AddRange(clients);
            _context.Cases.AddRange(cases);
            _context.Hearings.AddRange(hearings);
            _context.Tasks.AddRange(tasks);
            _context.Templates.AddRange(templates);
            _context.CaseFiles.AddRange(caseFiles);
            _context.Transactions.AddRange(txs);
            _context.CaseNotes.AddRange(notes);
            _context.Activities.AddRange(activities);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
