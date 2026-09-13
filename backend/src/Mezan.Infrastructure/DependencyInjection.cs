using System.Text;
using Mezan.Application.Common.Interfaces;
using Mezan.Infrastructure.Authentication;
using Mezan.Infrastructure.Persistence;
using Mezan.Infrastructure.Services;
using Mezan.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mezan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=mezan.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<DatabaseSeeder>();

        // JWT Authentication
        var secret = configuration["JwtSettings:Secret"] ?? "MezanSuperSecretKey_MustBeLongEnoughForHmacSha256_LegalLawFirmSecurity2025!";
        var issuer = configuration["JwtSettings:Issuer"] ?? "MezanApi";
        var audience = configuration["JwtSettings:Audience"] ?? "MezanApp";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
