using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class SEO : BaseEntity
{
    public string PageName { get; set; } = string.Empty; // e.g. Home, Blogs, Contact
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}
