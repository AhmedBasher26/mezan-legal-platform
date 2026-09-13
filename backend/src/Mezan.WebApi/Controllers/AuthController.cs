using Mezan.Application.DTOs;
using Mezan.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<LawyerDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUserAsync(cancellationToken);
        if (user == null) return Unauthorized();
        return Ok(user);
    }
}
