using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Resume : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
