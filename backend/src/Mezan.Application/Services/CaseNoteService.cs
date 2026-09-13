using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class CaseNoteService : ICaseNoteService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CaseNoteService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<CaseNoteDto>> GetNotesByCaseIdAsync(string caseId, CancellationToken cancellationToken = default)
    {
        var notes = await _context.CaseNotes
            .Where(n => n.CaseId == caseId && n.LawyerId == CurrentLawyerId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notes.Select(n => new CaseNoteDto(n.Id, n.LawyerId, n.CaseId, n.Text, n.CreatedAt)).ToList();
    }

    public async Task<CaseNoteDto> CreateNoteAsync(string caseId, CreateCaseNoteDto dto, CancellationToken cancellationToken = default)
    {
        var cs = await _context.Cases
            .FirstOrDefaultAsync(c => c.Id == caseId && c.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("القضية غير موجودة");

        var note = new CaseNote
        {
            LawyerId = CurrentLawyerId,
            CaseId = caseId,
            Text = dto.Text.Trim()
        };

        _context.CaseNotes.Add(note);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Note,
            Text = $"تمت إضافة ملاحظة على القضية {cs.CaseNumber}",
            CaseId = caseId
        });

        await _context.SaveChangesAsync(cancellationToken);
        return new CaseNoteDto(note.Id, note.LawyerId, note.CaseId, note.Text, note.CreatedAt);
    }
}
