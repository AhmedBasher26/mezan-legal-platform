using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mezan.Application.Common.Interfaces;
using Mezan.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mezan.Infrastructure.Authentication;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Lawyer lawyer)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? "MezanSuperSecretKey_MustBeLongEnoughForHmacSha256_LegalLawFirmSecurity2025!";
        var issuer = _configuration["JwtSettings:Issuer"] ?? "MezanApi";
        var audience = _configuration["JwtSettings:Audience"] ?? "MezanApp";
        var expiryDays = int.TryParse(_configuration["JwtSettings:ExpiryDays"], out var d) ? d : 30;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, lawyer.Id),
            new(ClaimTypes.Email, lawyer.Email),
            new(ClaimTypes.Name, lawyer.Name)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
