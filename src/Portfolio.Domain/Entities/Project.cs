using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ProjectUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty; // Comma-separated list
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
}
