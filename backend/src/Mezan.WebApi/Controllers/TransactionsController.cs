using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[Authorize]
public class TransactionsController : ApiControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions(
        [FromQuery] string? q,
        [FromQuery] string? type,
        CancellationToken cancellationToken)
    {
        var txs = await _transactionService.GetTransactionsAsync(q, type, cancellationToken);
        return Ok(txs);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<FinancialSummaryItemDto>>> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _transactionService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("totals")]
    public async Task<ActionResult<FinancialTotalsDto>> GetTotals(CancellationToken cancellationToken)
    {
        var totals = await _transactionService.GetTotalsAsync(cancellationToken);
        return Ok(totals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(string id, CancellationToken cancellationToken)
    {
        var tx = await _transactionService.GetTransactionByIdAsync(id, cancellationToken);
        if (tx == null) return NotFound(new { message = "العملية المالية غير موجودة" });
        return Ok(tx);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto dto, CancellationToken cancellationToken)
    {
        var tx = await _transactionService.CreateTransactionAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTransaction), new { id = tx.Id }, tx);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> UpdateTransaction(string id, [FromBody] UpdateTransactionDto dto, CancellationToken cancellationToken)
    {
        var tx = await _transactionService.UpdateTransactionAsync(id, dto, cancellationToken);
        return Ok(tx);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(string id, CancellationToken cancellationToken)
    {
        var result = await _transactionService.DeleteTransactionAsync(id, cancellationToken);
        if (!result) return NotFound(new { message = "العملية المالية غير موجودة" });
        return NoContent();
    }
}
