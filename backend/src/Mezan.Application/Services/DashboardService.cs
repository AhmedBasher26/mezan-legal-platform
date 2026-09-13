using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var tomorrow = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

        var clientCount = await _context.Clients.CountAsync(c => c.LawyerId == CurrentLawyerId, cancellationToken);
        var cases = await _context.Cases.Where(c => c.LawyerId == CurrentLawyerId).ToListAsync(cancellationToken);
        var tasks = await _context.Tasks.Where(t => t.LawyerId == CurrentLawyerId).ToListAsync(cancellationToken);
        var txs = await _context.Transactions.Where(t => t.LawyerId == CurrentLawyerId).ToListAsync(cancellationToken);

        var todayHearingsEntities = await _context.Hearings
            .Include(h => h.Case)
            .Where(h => h.LawyerId == CurrentLawyerId && h.Date == today)
            .OrderBy(h => h.Time)
            .ToListAsync(cancellationToken);

        var tomorrowHearingsEntities = await _context.Hearings
            .Include(h => h.Case)
            .Where(h => h.LawyerId == CurrentLawyerId && h.Date == tomorrow)
            .OrderBy(h => h.Time)
            .ToListAsync(cancellationToken);

        var fees = txs.Where(t => t.Type == TransactionType.Fees).Sum(t => t.Amount);
        var exp = txs.Where(t => t.Type == TransactionType.Expenses).Sum(t => t.Amount);

        var kpi = new DashboardKpiDto(
            Clients: clientCount,
            Cases: cases.Count,
            Open: cases.Count(c => c.Status == CaseStatus.Open),
            Postponed: cases.Count(c => c.Status == CaseStatus.Postponed),
            Closed: cases.Count(c => c.Status == CaseStatus.Closed),
            TodayHearings: todayHearingsEntities.Count,
            Pending: tasks.Count(t => !t.Completed),
            Overdue: tasks.Count(t => !t.Completed && t.Date.CompareTo(today) < 0),
            Fees: fees,
            Expenses: exp,
            Net: fees - exp
        );

        var todayHearings = todayHearingsEntities.Select(h =>
            new HearingDto(h.Id, h.LawyerId, h.CaseId, h.Date, h.Time, h.Type, h.Notes, h.CreatedAt)).ToList();

        var tomorrowHearings = tomorrowHearingsEntities.Select(h =>
            new HearingDto(h.Id, h.LawyerId, h.CaseId, h.Date, h.Time, h.Type, h.Notes, h.CreatedAt)).ToList();

        var todayTasks = tasks
            .Where(t => t.Date == today || (t.Date.CompareTo(today) < 0 && !t.Completed))
            .OrderBy(t => t.Date).ThenBy(t => t.Time)
            .Select(t => new TaskDto(
                t.Id, t.LawyerId, t.Title, t.Description, t.Date, t.Time,
                t.Priority.ToString().ToLowerInvariant(),
                t.CaseId, t.ClientId, t.Completed, t.CompletedAt, t.CreatedAt))
            .ToList();

        var activities = await _context.Activities
            .Where(a => a.LawyerId == CurrentLawyerId)
            .OrderByDescending(a => a.At)
            .Take(7)
            .Select(a => new ActivityDto(a.Id, a.LawyerId, a.Kind.ToString().ToLowerInvariant(), a.Text, a.CaseId, a.At))
            .ToListAsync(cancellationToken);

        return new DashboardSummaryDto(kpi, todayHearings, tomorrowHearings, todayTasks, activities);
    }
}
