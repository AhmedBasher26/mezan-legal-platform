using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class CaseFile : BaseEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string CaseId { get; set; } = string.Empty;
    public Case? Case { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Ext { get; set; } = string.Empty;
    public long Size { get; set; } // bytes
    public string? DataUrl { get; set; }
    public string? Content { get; set; }
    public string? TemplateId { get; set; }
    public string? StoragePath { get; set; }
    public long AddedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
