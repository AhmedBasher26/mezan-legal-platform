using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class CaseService : ICaseService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CaseService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<CaseDto>> GetCasesAsync(string? query = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Cases
            .Include(c => c.Client)
            .Where(c => c.LawyerId == CurrentLawyerId);

        if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
        {
            if (Enum.TryParse<CaseStatus>(status, true, out var parsedStatus))
            {
                q = q.Where(c => c.Status == parsedStatus);
            }
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLower();
            q = q.Where(c =>
                c.CaseNumber.ToLower().Contains(s) ||
                (c.Client != null && c.Client.Name.ToLower().Contains(s)) ||
                c.Court.ToLower().Contains(s));
        }

        var cases = await q.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
        return cases.Select(MapToDto).ToList();
    }

    public async Task<CaseDto?> GetCaseByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var cs = await _context.Cases
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken);

        return cs == null ? null : MapToDto(cs);
    }

    public async Task<bool> CaseNumberExistsAsync(string caseNumber, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var num = caseNumber.Trim().ToLower();
        return await _context.Cases
            .AnyAsync(c => c.LawyerId == CurrentLawyerId && c.CaseNumber.ToLower() == num && (excludeId == null || c.Id != excludeId), cancellationToken);
    }

    public async Task<CaseDto> CreateCaseAsync(CreateCaseDto dto, CancellationToken cancellationToken = default)
    {
        if (await CaseNumberExistsAsync(dto.CaseNumber, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("يوجد قضية مسجلة بهذا الرقم بالفعل");
        }

        Enum.TryParse<CaseStatus>(dto.Status, true, out var status);

        var cs = new Case
        {
            LawyerId = CurrentLawyerId,
            CaseNumber = dto.CaseNumber.Trim(),
            ClientId = dto.ClientId,
            Court = dto.Court.Trim(),
            CaseType = dto.CaseType.Trim(),
            FiledDate = dto.FiledDate.Trim(),
            NextHearingDate = string.IsNullOrWhiteSpace(dto.NextHearingDate) ? null : dto.NextHearingDate.Trim(),
            NextHearingTime = string.IsNullOrWhiteSpace(dto.NextHearingTime) ? null : dto.NextHearingTime.Trim(),
            Status = status,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        _context.Cases.Add(cs);

        // If next hearing date is provided, create a hearing record
        if (!string.IsNullOrEmpty(cs.NextHearingDate))
        {
            _context.Hearings.Add(new Hearing
            {
                LawyerId = CurrentLawyerId,
                CaseId = cs.Id,
                Date = cs.NextHearingDate,
                Time = cs.NextHearingTime ?? "09:00",
                Type = "جلسة نظر الدعوى"
            });
        }

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Case,
            Text = $"تم إنشاء قضية جديدة برقم {cs.CaseNumber}",
            CaseId = cs.Id
        });

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(cs);
    }

    public async Task<CaseDto> UpdateCaseAsync(string id, UpdateCaseDto dto, CancellationToken cancellationToken = default)
    {
        var cs = await _context.Cases
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("القضية غير موجودة");

        if (dto.CaseNumber != null && dto.CaseNumber.Trim() != cs.CaseNumber)
        {
            if (await CaseNumberExistsAsync(dto.CaseNumber, id, cancellationToken))
            {
                throw new InvalidOperationException("يوجد قضية مسجلة بهذا الرقم بالفعل");
            }
            cs.CaseNumber = dto.CaseNumber.Trim();
        }

        if (dto.ClientId != null) cs.ClientId = dto.ClientId;
        if (dto.Court != null) cs.Court = dto.Court.Trim();
        if (dto.CaseType != null) cs.CaseType = dto.CaseType.Trim();
        if (dto.FiledDate != null) cs.FiledDate = dto.FiledDate.Trim();

        var prevHearingDate = cs.NextHearingDate;
        if (dto.NextHearingDate != null) cs.NextHearingDate = string.IsNullOrWhiteSpace(dto.NextHearingDate) ? null : dto.NextHearingDate.Trim();
        if (dto.NextHearingTime != null) cs.NextHearingTime = string.IsNullOrWhiteSpace(dto.NextHearingTime) ? null : dto.NextHearingTime.Trim();

        if (dto.Status != null && Enum.TryParse<CaseStatus>(dto.Status, true, out var parsedStatus))
        {
            cs.Status = parsedStatus;
        }

        if (dto.Description != null) cs.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        if (dto.Notes != null) cs.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

        cs.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // If next hearing date changed to a new date, create a hearing record
        if (!string.IsNullOrEmpty(cs.NextHearingDate) && cs.NextHearingDate != prevHearingDate)
        {
            _context.Hearings.Add(new Hearing
            {
                LawyerId = CurrentLawyerId,
                CaseId = cs.Id,
                Date = cs.NextHearingDate,
                Time = cs.NextHearingTime ?? "09:00",
                Type = "جلسة نظر الدعوى"
            });
        }

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Case,
            Text = $"تم تحديث القضية {cs.CaseNumber}",
            CaseId = cs.Id
        });

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(cs);
    }

    public async Task<bool> DeleteCaseAsync(string id, CancellationToken cancellationToken = default)
    {
        var cs = await _context.Cases
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken);

        if (cs == null) return false;

        var hearings = await _context.Hearings.Where(h => h.CaseId == id).ToListAsync(cancellationToken);
        _context.Hearings.RemoveRange(hearings);

        var notes = await _context.CaseNotes.Where(n => n.CaseId == id).ToListAsync(cancellationToken);
        _context.CaseNotes.RemoveRange(notes);

        var files = await _context.CaseFiles.Where(f => f.CaseId == id).ToListAsync(cancellationToken);
        _context.CaseFiles.RemoveRange(files);

        var txs = await _context.Transactions.Where(t => t.CaseId == id).ToListAsync(cancellationToken);
        _context.Transactions.RemoveRange(txs);

        var tasks = await _context.Tasks.Where(t => t.CaseId == id).ToListAsync(cancellationToken);
        foreach (var task in tasks) task.CaseId = null;

        _context.Cases.Remove(cs);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Case,
            Text = $"تم حذف القضية رقم {cs.CaseNumber}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CaseDto MapToDto(Case c) =>
        new(
            c.Id,
            c.LawyerId,
            c.CaseNumber,
            c.ClientId,
            c.Court,
            c.CaseType,
            c.FiledDate,
            c.NextHearingDate,
            c.NextHearingTime,
            c.Status.ToString().ToLowerInvariant(),
            c.Description,
            c.Notes,
            c.CreatedAt
        );
}
