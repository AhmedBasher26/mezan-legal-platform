using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Mezan.UnitTests;

public class ClientServiceTests : TestBase
{
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        _service = new ClientService(Context, CurrentUserService);
    }

    [Fact]
    public async Task CreateClientAsync_ValidData_CreatesClient()
    {
        // Arrange
        var dto = new CreateClientDto("شركة المستقبل", "0512345678", "info@future.sa", "7009999999", "الرياض", "ملاحظة");

        // Act
        var created = await _service.CreateClientAsync(dto);

        // Assert
        Assert.NotNull(created);
        Assert.Equal("شركة المستقبل", created.Name);
        Assert.Equal(TestLawyerId, created.LawyerId);

        var fromDb = await Context.Clients.FindAsync(created.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("info@future.sa", fromDb.Email);
    }

    [Fact]
    public async Task AddImportantDateAsync_ValidData_AddsDate()
    {
        // Arrange
        var client = await _service.CreateClientAsync(new CreateClientDto("عميل تجربة", "0500000001", "c@test.com", null, null, null));
        var dateDto = new CreateImportantDateDto("تجديد السجل", "2026-10-15");

        // Act
        var result = await _service.AddImportantDateAsync(client.Id, dateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("تجديد السجل", result.Label);

        var dates = await Context.ImportantDates.Where(d => d.ClientId == client.Id).ToListAsync();
        Assert.Single(dates);
    }

    [Fact]
    public async Task DeleteClientAsync_RemovesClient()
    {
        // Arrange
        var client = await _service.CreateClientAsync(new CreateClientDto("عميل للحذف", "0500000002", "del@test.com", null, null, null));

        // Act
        var deleted = await _service.DeleteClientAsync(client.Id);

        // Assert
        Assert.True(deleted);
        var fromDb = await Context.Clients.FindAsync(client.Id);
        Assert.Null(fromDb);
    }
}
