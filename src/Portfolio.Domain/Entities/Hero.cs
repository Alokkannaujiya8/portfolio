using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Hero : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = string.Empty;
    public string SecondaryButtonText { get; set; } = string.Empty;
}
