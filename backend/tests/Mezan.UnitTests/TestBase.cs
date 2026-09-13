using Mezan.Application.Common.Interfaces;
using Mezan.Domain.Entities;
using Mezan.Infrastructure.Authentication;
using Mezan.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Mezan.UnitTests;

public abstract class TestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly ApplicationDbContext Context;
    protected readonly IPasswordHasher PasswordHasher;
    protected readonly ITokenService TokenService;
    protected readonly MockCurrentUserService CurrentUserService;
    protected readonly string TestLawyerId = "test-lawyer-123";

    protected TestBase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();

        PasswordHasher = new PasswordHasher();
        CurrentUserService = new MockCurrentUserService { LawyerId = TestLawyerId };

        // Mock token service
        TokenService = new MockTokenService();

        // Seed a test lawyer
        var testLawyer = new Lawyer
        {
            Id = TestLawyerId,
            Name = "محامي الاختبار",
            Email = "test@mezan.sa",
            Phone = "0511111111",
            PasswordHash = PasswordHasher.HashPassword("password123")
        };
        Context.Lawyers.Add(testLawyer);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

public class MockCurrentUserService : ICurrentUserService
{
    public string? LawyerId { get; set; }
    public string? Email { get; set; } = "test@mezan.sa";
    public bool IsAuthenticated => LawyerId != null;
}

public class MockTokenService : ITokenService
{
    public string GenerateToken(Lawyer lawyer) => "test-jwt-token-" + lawyer.Id;
}
