using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Frontend, Backend, Database, Cloud, Soft Skills, etc.
    public int Proficiency { get; set; } // 0 to 100
    public int DisplayOrder { get; set; }
}
