using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class HearingService : IHearingService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public HearingService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<HearingDto>> GetHearingsByCaseIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        var hearings = await _context.Hearings
            .Where(h => h.CaseId == caseId && h.LawyerId == CurrentLawyerId)
            .OrderBy(h => h.Date).ThenBy(h => h.Time)
            .ToListAsync(cancellationToken);

        return hearings.Select(MapToDto).ToList();
    }

    public async Task<List<HearingDto>> GetAgendaHearingsAsync(string? startDate = null, string? endDate = null, CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
        var end = endDate ?? DateTime.UtcNow.AddDays(6).ToString("yyyy-MM-dd");

        var hearings = await _context.Hearings
            .Include(h => h.Case)
            .Where(h => h.LawyerId == CurrentLawyerId && h.Date.CompareTo(start) >= 0 && h.Date.CompareTo(end) <= 0)
            .OrderBy(h => h.Date).ThenBy(h => h.Time)
            .ToListAsync(cancellationToken);

        return hearings.Select(MapToDto).ToList();
    }

    public async Task<HearingDto> CreateHearingAsync(string caseId, CreateHearingDto dto, CancellationToken cancellationToken = default)
    {
        var cs = await _context.Cases
            .FirstOrDefaultAsync(c => c.Id == caseId && c.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("القضية غير موجودة");

        var hearing = new Hearing
        {
            LawyerId = CurrentLawyerId,
            CaseId = caseId,
            Date = dto.Date.Trim(),
            Time = dto.Time.Trim(),
            Type = dto.Type.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        _context.Hearings.Add(hearing);

        // Update parent case's next hearing if applicable
        await UpdateCaseNextHearingAsync(cs, cancellationToken);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Hearing,
            Text = $"تم تحديد جلسة {hearing.Type} في القضية {cs.CaseNumber}",
            CaseId = cs.Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(hearing);
    }

    public async Task<bool> DeleteHearingAsync(string id, CancellationToken cancellationToken = default)
    {
        var hearing = await _context.Hearings
            .FirstOrDefaultAsync(h => h.Id == id && h.LawyerId == CurrentLawyerId, cancellationToken);

        if (hearing == null) return false;

        var cs = await _context.Cases.FirstOrDefaultAsync(c => c.Id == hearing.CaseId, cancellationToken);

        _context.Hearings.Remove(hearing);

        if (cs != null)
        {
            await UpdateCaseNextHearingAsync(cs, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task UpdateCaseNextHearingAsync(Case cs, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var upcoming = await _context.Hearings
            .Where(h => h.CaseId == cs.Id && h.Date.CompareTo(today) >= 0)
            .OrderBy(h => h.Date).ThenBy(h => h.Time)
            .FirstOrDefaultAsync(cancellationToken);

        cs.NextHearingDate = upcoming?.Date;
        cs.NextHearingTime = upcoming?.Time;
    }

    private static HearingDto MapToDto(Hearing h) =>
        new(h.Id, h.LawyerId, h.CaseId, h.Date, h.Time, h.Type, h.Notes, h.CreatedAt);
}
