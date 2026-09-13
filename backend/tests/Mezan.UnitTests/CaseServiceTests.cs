using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Mezan.UnitTests;

public class CaseServiceTests : TestBase
{
    private readonly CaseService _caseService;
    private readonly ClientService _clientService;

    public CaseServiceTests()
    {
        _caseService = new CaseService(Context, CurrentUserService);
        _clientService = new ClientService(Context, CurrentUserService);
    }

    [Fact]
    public async Task CreateCaseAsync_WithNextHearing_CreatesHearingAutomatically()
    {
        // Arrange
        var client = await _clientService.CreateClientAsync(new CreateClientDto("موكل قضية", "0500000003", "case@test.com", null, null, null));
        var caseDto = new CreateCaseDto(
            CaseNumber: "999/2026",
            ClientId: client.Id,
            Court: "المحكمة التجارية",
            CaseType: "تجارية",
            FiledDate: "2026-09-01",
            NextHearingDate: "2026-09-20",
            NextHearingTime: "10:30",
            Status: "open",
            Description: "دعوى تجارية",
            Notes: ""
        );

        // Act
        var created = await _caseService.CreateCaseAsync(caseDto);

        // Assert
        Assert.NotNull(created);
        Assert.Equal("999/2026", created.CaseNumber);

        var hearings = await Context.Hearings.Where(h => h.CaseId == created.Id).ToListAsync();
        Assert.Single(hearings);
        Assert.Equal("2026-09-20", hearings[0].Date);
        Assert.Equal("10:30", hearings[0].Time);
    }

    [Fact]
    public async Task CreateCaseAsync_WithDuplicateNumber_ThrowsInvalidOperationException()
    {
        // Arrange
        var client = await _clientService.CreateClientAsync(new CreateClientDto("موكل 2", "0500000004", "case2@test.com", null, null, null));
        var caseDto1 = new CreateCaseDto("555/2026", client.Id, "المحكمة العامة", "عقارية", "2026-09-01", null, null, "open", null, null);
        var caseDto2 = new CreateCaseDto("555/2026", client.Id, "المحكمة العامة", "عقارية", "2026-09-01", null, null, "open", null, null);

        // Act
        await _caseService.CreateCaseAsync(caseDto1);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _caseService.CreateCaseAsync(caseDto2));
    }
}
