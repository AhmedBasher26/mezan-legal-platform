using Mezan.Application.DTOs;
using Mezan.Application.Services;

namespace Mezan.UnitTests;

public class TransactionServiceTests : TestBase
{
    private readonly TransactionService _txService;
    private readonly ClientService _clientService;

    public TransactionServiceTests()
    {
        _txService = new TransactionService(Context, CurrentUserService);
        _clientService = new ClientService(Context, CurrentUserService);
    }

    [Fact]
    public async Task GetTotalsAsync_CalculatesFeesExpensesAndNetCorrectly()
    {
        // Arrange
        var client = await _clientService.CreateClientAsync(new CreateClientDto("موكل مالية", "0500000005", "finance@test.com", null, null, null));

        await _txService.CreateTransactionAsync(new CreateTransactionDto(client.Id, null, 15000, "fees", "2026-09-01", "أتعاب"));
        await _txService.CreateTransactionAsync(new CreateTransactionDto(client.Id, null, 5000, "fees", "2026-09-05", "دفعة ثانية"));
        await _txService.CreateTransactionAsync(new CreateTransactionDto(client.Id, null, 2000, "expenses", "2026-09-06", "رسوم"));

        // Act
        var totals = await _txService.GetTotalsAsync();

        // Assert
        Assert.Equal(20000, totals.TotalFees);
        Assert.Equal(2000, totals.TotalExpenses);
        Assert.Equal(18000, totals.NetIncome);
    }
}
