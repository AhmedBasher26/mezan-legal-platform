using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TransactionService(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private string CurrentLawyerId =>
        _currentUserService.LawyerId ?? throw new UnauthorizedAccessException("غير مصرح بالدخول");

    public async Task<List<TransactionDto>> GetTransactionsAsync(string? query = null, string? type = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Transactions
            .Include(t => t.Client)
            .Include(t => t.Case)
            .Where(t => t.LawyerId == CurrentLawyerId);

        if (!string.IsNullOrWhiteSpace(type) && type != "all" && Enum.TryParse<TransactionType>(type, true, out var tType))
        {
            q = q.Where(t => t.Type == tType);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim().ToLower();
            q = q.Where(t =>
                (t.Client != null && t.Client.Name.ToLower().Contains(s)) ||
                (t.Case != null && t.Case.CaseNumber.ToLower().Contains(s)) ||
                (t.Notes != null && t.Notes.ToLower().Contains(s)));
        }

        var txs = await q.OrderByDescending(t => t.Date).ThenByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);
        return txs.Select(MapToDto).ToList();
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var tx = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        return tx == null ? null : MapToDto(tx);
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        Enum.TryParse<TransactionType>(dto.Type, true, out var type);

        var tx = new Transaction
        {
            LawyerId = CurrentLawyerId,
            ClientId = dto.ClientId,
            CaseId = string.IsNullOrWhiteSpace(dto.CaseId) ? null : dto.CaseId,
            Amount = dto.Amount,
            Type = type,
            Date = dto.Date.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        _context.Transactions.Add(tx);

        string caseNum = "";
        if (tx.CaseId != null)
        {
            var cs = await _context.Cases.FindAsync(new object[] { tx.CaseId }, cancellationToken);
            if (cs != null) caseNum = cs.CaseNumber;
        }

        var typeLabel = tx.Type == TransactionType.Fees ? "أتعاب" : "مصروفات";
        var forCase = !string.IsNullOrEmpty(caseNum) ? $" للقضية {caseNum}" : "";

        _context.Activities.Add(new Activity
        {
            LawyerId = CurrentLawyerId,
            Kind = ActivityKind.Finance,
            Text = $"تم تسجيل {typeLabel} {tx.Amount:N0} ر.س{forCase}",
            CaseId = tx.CaseId
        });

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(tx);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(string id, UpdateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var tx = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken)
            ?? throw new KeyNotFoundException("العملية المالية غير موجودة");

        if (dto.ClientId != null) tx.ClientId = dto.ClientId;
        if (dto.CaseId != null) tx.CaseId = string.IsNullOrWhiteSpace(dto.CaseId) ? null : dto.CaseId;
        if (dto.Amount.HasValue) tx.Amount = dto.Amount.Value;
        if (dto.Type != null && Enum.TryParse<TransactionType>(dto.Type, true, out var type)) tx.Type = type;
        if (dto.Date != null) tx.Date = dto.Date.Trim();
        if (dto.Notes != null) tx.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

        tx.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(tx);
    }

    public async Task<bool> DeleteTransactionAsync(string id, CancellationToken cancellationToken = default)
    {
        var tx = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.LawyerId == CurrentLawyerId, cancellationToken);

        if (tx == null) return false;

        _context.Transactions.Remove(tx);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<FinancialSummaryItemDto>> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var txs = await _context.Transactions
            .Where(t => t.LawyerId == CurrentLawyerId)
            .ToListAsync(cancellationToken);

        var grouped = txs
            .GroupBy(t => new { t.ClientId, t.CaseId })
            .Select(g =>
            {
                var fees = g.Where(x => x.Type == TransactionType.Fees).Sum(x => x.Amount);
                var exp = g.Where(x => x.Type == TransactionType.Expenses).Sum(x => x.Amount);
                return new FinancialSummaryItemDto(g.Key.ClientId, g.Key.CaseId, fees, exp, fees - exp);
            })
            .OrderByDescending(s => s.Net)
            .ToList();

        return grouped;
    }

    public async Task<FinancialTotalsDto> GetTotalsAsync(CancellationToken cancellationToken = default)
    {
        var txs = await _context.Transactions
            .Where(t => t.LawyerId == CurrentLawyerId)
            .ToListAsync(cancellationToken);

        var fees = txs.Where(x => x.Type == TransactionType.Fees).Sum(x => x.Amount);
        var exp = txs.Where(x => x.Type == TransactionType.Expenses).Sum(x => x.Amount);

        return new FinancialTotalsDto(fees, exp, fees - exp);
    }

    private static TransactionDto MapToDto(Transaction t) =>
        new(
            t.Id,
            t.LawyerId,
            t.ClientId,
            t.CaseId,
            t.Amount,
            t.Type.ToString().ToLowerInvariant(),
            t.Date,
            t.Notes,
            t.CreatedAt
        );
}
