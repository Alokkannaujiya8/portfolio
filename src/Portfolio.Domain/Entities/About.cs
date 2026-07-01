using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class About : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int ProjectsCompleted { get; set; }
}
