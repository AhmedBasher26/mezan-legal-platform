namespace Mezan.Application.DTOs;

public record TransactionDto(
    string Id,
    string LawyerId,
    string ClientId,
    string? CaseId,
    decimal Amount,
    string Type,
    string Date,
    string? Notes,
    long CreatedAt
);

public record CreateTransactionDto(
    string ClientId,
    string? CaseId,
    decimal Amount,
    string Type,
    string Date,
    string? Notes
);

public record UpdateTransactionDto(
    string? ClientId,
    string? CaseId,
    decimal? Amount,
    string? Type,
    string? Date,
    string? Notes
);

public record FinancialSummaryItemDto(
    string ClientId,
    string? CaseId,
    decimal Fees,
    decimal Expenses,
    decimal Net
);

public record FinancialTotalsDto(
    decimal TotalFees,
    decimal TotalExpenses,
    decimal NetIncome
);
