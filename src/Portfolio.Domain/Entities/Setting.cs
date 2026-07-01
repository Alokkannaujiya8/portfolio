using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Setting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
