using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class GalleryItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g. Projects, Certificates, Workspace, Events
}
