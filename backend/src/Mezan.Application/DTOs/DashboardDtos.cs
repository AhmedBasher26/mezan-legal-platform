namespace Mezan.Application.DTOs;

public record DashboardKpiDto(
    int Clients,
    int Cases,
    int Open,
    int Postponed,
    int Closed,
    int TodayHearings,
    int Pending,
    int Overdue,
    decimal Fees,
    decimal Expenses,
    decimal Net
);

public record DashboardSummaryDto(
    DashboardKpiDto Kpi,
    List<HearingDto> TodayHearings,
    List<HearingDto> TomorrowHearings,
    List<TaskDto> TodayTasks,
    List<ActivityDto> RecentActivities
);
