using Mezan.Application.DTOs;

namespace Mezan.Application.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetTransactionsAsync(string? query = null, string? type = null, CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetTransactionByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default);
    Task<TransactionDto> UpdateTransactionAsync(string id, UpdateTransactionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTransactionAsync(string id, CancellationToken cancellationToken = default);
    Task<List<FinancialSummaryItemDto>> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<FinancialTotalsDto> GetTotalsAsync(CancellationToken cancellationToken = default);
}
