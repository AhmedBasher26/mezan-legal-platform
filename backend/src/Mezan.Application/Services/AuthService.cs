using Mezan.Application.Common;
using Mezan.Application.Common.Interfaces;
using Mezan.Application.DTOs;
using Mezan.Domain.Entities;
using Mezan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mezan.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;

    public AuthService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var idn = request.Identifier.Trim().ToLower();
        var lawyer = await _context.Lawyers
            .FirstOrDefaultAsync(l => l.Email.ToLower() == idn || l.Name.ToLower() == idn, cancellationToken);

        if (lawyer == null || !_passwordHasher.VerifyPassword(request.Password, lawyer.PasswordHash))
        {
            throw new UnauthorizedAccessException("بيانات الدخول غير صحيحة، تأكد من البريد الإلكتروني وكلمة المرور.");
        }

        // Log login activity
        var activity = new Activity
        {
            LawyerId = lawyer.Id,
            Kind = ActivityKind.Auth,
            Text = "تم تسجيل الدخول",
            At = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(lawyer);
        var userDto = new LawyerDto(lawyer.Id, lawyer.Name, lawyer.Email, lawyer.Phone, lawyer.CreatedAt);

        return new AuthResponse(token, userDto);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLower();
        var exists = await _context.Lawyers.AnyAsync(l => l.Email.ToLower() == email, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("هذا البريد الإلكتروني مسجل مسبقاً، جرّب تسجيل الدخول.");
        }

        var lawyer = new Lawyer
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        _context.Lawyers.Add(lawyer);

        if (request.WithDemo)
        {
            var (clients, cases, hearings, tasks, templates, caseFiles, txs, notes, activities) =
                SeedDataBuilder.BuildDemoData(lawyer.Id);

            _context.Clients.AddRange(clients);
            _context.Cases.AddRange(cases);
            _context.Hearings.AddRange(hearings);
            _context.Tasks.AddRange(tasks);
            _context.Templates.AddRange(templates);
            _context.CaseFiles.AddRange(caseFiles);
            _context.Transactions.AddRange(txs);
            _context.CaseNotes.AddRange(notes);
            _context.Activities.AddRange(activities);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(lawyer);
        var userDto = new LawyerDto(lawyer.Id, lawyer.Name, lawyer.Email, lawyer.Phone, lawyer.CreatedAt);

        return new AuthResponse(token, userDto);
    }

    public async Task<LawyerDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var currentLawyerId = _currentUserService.LawyerId;
        if (string.IsNullOrEmpty(currentLawyerId)) return null;

        var lawyer = await _context.Lawyers.FindAsync(new object[] { currentLawyerId }, cancellationToken);
        if (lawyer == null) return null;

        return new LawyerDto(lawyer.Id, lawyer.Name, lawyer.Email, lawyer.Phone, lawyer.CreatedAt);
    }
}
