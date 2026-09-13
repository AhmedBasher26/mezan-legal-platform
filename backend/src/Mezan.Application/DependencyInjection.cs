using Mezan.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mezan.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IHearingService, HearingService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<ICaseFileService, CaseFileService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ICaseNoteService, CaseNoteService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
