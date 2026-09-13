using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class CaseFileService : ICaseFileService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CaseFileService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<CaseFileDto>> GetCaseFilesAsync(string caseId, CancellationToken cancellationToken = default)
    {
        var files = await _context.CaseFiles
            .Where(f => f.CaseId == caseId && f.LawyerId == CurrentLawyerId)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync(cancellationToken);

        return files.Select(MapToDto).ToList();
    }

    public async Task<CaseFileDto> CreateCaseFileAsync(string caseId, CreateCaseFileDto dto, CancellationToken cancellationToken = default)
    {
        var file = new CaseFile
        {
            LawyerId = CurrentLawyerId,
            CaseId = caseId,
            Name = dto.Name.Trim(),
            Ext = dto.Ext.Trim().ToLower(),
            Size = dto.Size,
            DataUrl = dto.DataUrl,
            Content = dto.Content,
            TemplateId = dto.TemplateId,
            AddedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.CaseFiles.Add(file);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(file);
    }

    public async Task<bool> DeleteCaseFileAsync(string id, CancellationToken cancellationToken = default)
    {
        var file = await _context.CaseFiles
            .FirstOrDefaultAsync(f => f.Id == id && f.LawyerId == CurrentLawyerId, cancellationToken);

        if (file == null) return false;

        _context.CaseFiles.Remove(file);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CaseFileDto MapToDto(CaseFile f) =>
        new(
            f.Id,
            f.LawyerId,
            f.CaseId,
            f.Name,
            f.Ext,
            f.Size,
            f.DataUrl,
            f.Content,
            f.TemplateId,
            f.AddedAt
        );
}
