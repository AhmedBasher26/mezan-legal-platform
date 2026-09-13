namespace Mezan.Domain.Enums;

public enum CaseStatus
{
    Open,
    Postponed,
    Closed
}

public enum Priority
{
    Low,
    Medium,
    High
}

public enum TransactionType
{
    Fees,
    Expenses
}

public enum ActivityKind
{
    Client,
    Case,
    Task,
    Finance,
    Template,
    Hearing,
    Note,
    Auth
}

public enum TemplateKind
{
    Builtin,
    File
}
