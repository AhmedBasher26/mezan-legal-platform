using Mezan.Domain.Common;

namespace Mezan.Domain.Entities;

public class ImportantDate : BaseEntity
{
    public string ClientId { get; set; } = string.Empty;
    public Client? Client { get; set; }

    public string Label { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty; // YYYY-MM-DD
}
