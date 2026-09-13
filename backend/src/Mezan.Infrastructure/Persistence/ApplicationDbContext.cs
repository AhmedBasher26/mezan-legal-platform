using Mezan.Application.Common.Interfaces;
using Mezan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Lawyer> Lawyers => Set<Lawyer>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ImportantDate> ImportantDates => Set<ImportantDate>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Hearing> Hearings => Set<Hearing>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<CaseFile> CaseFiles => Set<CaseFile>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Lawyer
        modelBuilder.Entity<Lawyer>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.Name).IsRequired().HasMaxLength(150);
            b.Property(x => x.Email).IsRequired().HasMaxLength(150);
            b.Property(x => x.Phone).HasMaxLength(50);
        });

        // Client
        modelBuilder.Entity<Client>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Clients)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ImportantDates)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ImportantDate
        modelBuilder.Entity<ImportantDate>(b =>
        {
            b.HasKey(x => x.Id);
        });

        // Case
        modelBuilder.Entity<Case>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.LawyerId, x.CaseNumber });

            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Cases)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Client)
                .WithMany(c => c.Cases)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Hearings)
                .WithOne(h => h.Case)
                .HasForeignKey(h => h.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.CaseFiles)
                .WithOne(f => f.Case)
                .HasForeignKey(f => f.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.NotesList)
                .WithOne(n => n.Case)
                .HasForeignKey(n => n.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Hearing
        modelBuilder.Entity<Hearing>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Hearings)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Tasks)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Case)
                .WithMany(c => c.Tasks)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Client)
                .WithMany(c => c.Tasks)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Template
        modelBuilder.Entity<Template>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Templates)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CaseFile
        modelBuilder.Entity<CaseFile>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.CaseFiles)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction
        modelBuilder.Entity<Transaction>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Amount).HasColumnType("decimal(18,2)");

            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Transactions)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Client)
                .WithMany(c => c.Transactions)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Case)
                .WithMany(c => c.Transactions)
                .HasForeignKey(x => x.CaseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CaseNote
        modelBuilder.Entity<CaseNote>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.CaseNotes)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Activity
        modelBuilder.Entity<Activity>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Lawyer)
                .WithMany(l => l.Activities)
                .HasForeignKey(x => x.LawyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
