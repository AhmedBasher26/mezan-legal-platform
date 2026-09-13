using Mezan.Application.DTOs;
using Mezan.Application.Services;

namespace Mezan.UnitTests;

public class AuthServiceTests : TestBase
{
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(Context, PasswordHasher, TokenService, CurrentUserService);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Act
        var result = await _service.LoginAsync(new LoginRequest("test@mezan.sa", "password123"));

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("test@mezan.sa", result.User.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.LoginAsync(new LoginRequest("test@mezan.sa", "wrongPassword")));
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesLawyerAndReturnsToken()
    {
        // Arrange
        var request = new RegisterRequest("محامٍ جديد", "newlawyer@mezan.sa", "0599999999", "secret123", WithDemo: false);

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newlawyer@mezan.sa", result.User.Email);
        Assert.Equal("محامٍ جديد", result.User.Name);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest("مكرر", "test@mezan.sa", "0599999999", "secret123", WithDemo: false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegisterAsync(request));
    }
}
