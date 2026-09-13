using Mezan.Domain.Common;
using Mezan.Domain.Enums;

namespace Mezan.Domain.Entities;

public class Template : BaseEntity
{
    public string LawyerId { get; set; } = string.Empty;
    public Lawyer? Lawyer { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Ext { get; set; } = string.Empty; // pdf | doc | docx
    public long Size { get; set; } // bytes
    public TemplateKind Kind { get; set; } = TemplateKind.File;
    public string? Content { get; set; } // HTML content for builtin templates
    public string? DataUrl { get; set; } // Base64 dataUrl or storage url
    public string? StoragePath { get; set; }
    public long AddedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
