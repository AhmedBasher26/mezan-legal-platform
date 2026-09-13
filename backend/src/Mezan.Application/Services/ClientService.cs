using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class ClientService : IClientService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ClientService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<ClientDto>> GetClientsAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Clients
            .Include(c => c.ImportantDates)
            .Where(c => c.LawyerId == CurrentLawyerId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLower();
            q = q.Where(c =>
                c.Name.ToLower().Contains(s) ||
                c.Phone.ToLower().Contains(s) ||
                (c.Email != null && c.Email.ToLower().Contains(s)) ||
                c.Cases.Any(cs => cs.CaseNumber.ToLower().Contains(s)));
        }

        var clients = await q.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);

        return clients.Select(MapToDto).ToList();
    }

    public async Task<ClientDto?> GetClientByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .Include(c => c.ImportantDates)
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken);

        return client == null ? null : MapToDto(client);
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto dto, CancellationToken cancellationToken = default)
    {
        var client = new Client
        {
            LawyerId = CurrentLawyerId,
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim(),
            NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim(),
            Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        _context.Clients.Add(client);

        // Activity log
        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Client,
            Text = $"تمت إضافة موكل جديد: {client.Name}"
        });

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(client);
    }

    public async Task<ClientDto> UpdateClientAsync(string id, UpdateClientDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .Include(c => c.ImportantDates)
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("الموكل غير موجود");

        if (dto.Name != null) client.Name = dto.Name.Trim();
        if (dto.Phone != null) client.Phone = dto.Phone.Trim();
        if (dto.Email != null) client.Email = dto.Email.Trim();
        if (dto.NationalId != null) client.NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim();
        if (dto.Address != null) client.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        if (dto.Notes != null) client.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

        client.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(client);
    }

    public async Task<bool> DeleteClientAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id && c.LawyerId == CurrentLawyerId, cancellationToken);

        if (client == null) return false;

        var clientCases = await _context.Cases
            .Where(c => c.ClientId == id && c.LawyerId == CurrentLawyerId)
            .ToListAsync(cancellationToken);

        var caseIds = clientCases.Select(c => c.Id).ToList();

        // Delete hearings for these cases
        var hearings = await _context.Hearings.Where(h => caseIds.Contains(h.CaseId)).ToListAsync(cancellationToken);
        _context.Hearings.RemoveRange(hearings);

        // Delete notes for these cases
        var notes = await _context.CaseNotes.Where(n => caseIds.Contains(n.CaseId)).ToListAsync(cancellationToken);
        _context.CaseNotes.RemoveRange(notes);

        // Delete files for these cases
        var files = await _context.CaseFiles.Where(f => caseIds.Contains(f.CaseId)).ToListAsync(cancellationToken);
        _context.CaseFiles.RemoveRange(files);

        // Nullify task references
        var tasks = await _context.Tasks
            .Where(t => t.ClientId == id || (t.CaseId != null && caseIds.Contains(t.CaseId)))
            .ToListAsync(cancellationToken);

        foreach (var task in tasks)
        {
            if (task.ClientId == id) task.ClientId = null;
            if (task.CaseId != null && caseIds.Contains(task.CaseId)) task.CaseId = null;
        }

        // Delete transactions
        var txs = await _context.Transactions.Where(t => t.ClientId == id).ToListAsync(cancellationToken);
        _context.Transactions.RemoveRange(txs);

        // Delete cases
        _context.Cases.RemoveRange(clientCases);

        // Delete client
        _context.Clients.Remove(client);

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Client,
            Text = $"تم حذف الموكل «{client.Name}» و{caseIds.Count} قضية مرتبطة به"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ImportantDateDto> AddImportantDateAsync(string clientId, CreateImportantDateDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId && c.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("الموكل غير موجود");

        var date = new ImportantDate
        {
            ClientId = client.Id,
            Label = dto.Label.Trim(),
            Date = dto.Date.Trim()
        };

        _context.ImportantDates.Add(date);
        await _context.SaveChangesAsync(cancellationToken);

        return new ImportantDateDto(date.Id, date.Label, date.Date);
    }

    public async Task<bool> DeleteImportantDateAsync(string clientId, string dateId, CancellationToken cancellationToken = default)
    {
        var date = await _context.ImportantDates
            .FirstOrDefaultAsync(d => d.Id == dateId && d.ClientId == clientId && d.Client!.LawyerId == CurrentLawyerId, cancellationToken);

        if (date == null) return false;

        _context.ImportantDates.Remove(date);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ClientDto MapToDto(Client c) =>
        new(
            c.Id,
            c.LawyerId,
            c.Name,
            c.Phone,
            c.Email,
            c.NationalId,
            c.Address,
            c.Notes,
            c.ImportantDates.Select(d => new ImportantDateDto(d.Id, d.Label, d.Date)).ToList(),
            c.CreatedAt
        );
}
