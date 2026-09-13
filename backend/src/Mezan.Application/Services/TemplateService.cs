using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class TemplateService : ITemplateService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TemplateService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<TemplateDto>> GetTemplatesAsync(string? query = null, string? ext = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Templates.Where(t => t.LawyerId == CurrentLawyerId);

        if (!string.IsNullOrWhiteSpace(ext) && ext != "all")
        {
            q = q.Where(t => t.Ext.ToLower() == ext.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLower();
            q = q.Where(t => t.Name.ToLower().Contains(s));
        }

        var templates = await q.OrderByDescending(t => t.AddedAt).ToListAsync(cancellationToken);
        return templates.Select(MapToDto).ToList();
    }

    public async Task<TemplateDto?> GetTemplateByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var template = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        return template == null ? null : MapToDto(template);
    }

    public async Task<TemplateDto> CreateTemplateAsync(CreateTemplateDto dto, CancellationToken cancellationToken = default)
    {
        Enum.TryParse<TemplateKind>(dto.Kind, true, out var kind);

        var template = new Template
        {
            LawyerId = CurrentLawyerId,
            Name = dto.Name.Trim(),
            Ext = dto.Ext.Trim().ToLower(),
            Size = dto.Size,
            Kind = kind,
            Content = dto.Content,
            DataUrl = dto.DataUrl,
            AddedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _context.Templates.Add(template);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Template,
            Text = $"تم رفع نموذج جديد: {template.Name}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(template);
    }

    public async Task<TemplateDto> RenameTemplateAsync(string id, RenameTemplateDto dto, CancellationToken cancellationToken = default)
    {
        var template = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("النموذج غير موجود");

        template.Name = dto.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(template);
    }

    public async Task<bool> DeleteTemplateAsync(string id, CancellationToken cancellationToken = default)
    {
        var template = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        if (template == null) return false;

        _context.Templates.Remove(template);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Template,
            Text = $"تم حذف النموذج «{template.Name}»"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TemplateDto MapToDto(Template t) =>
        new(
            t.Id,
            t.LawyerId,
            t.Name,
            t.Ext,
            t.Size,
            t.Kind.ToString().ToLowerInvariant(),
            t.Content,
            t.DataUrl,
            t.AddedAt
        );
}
