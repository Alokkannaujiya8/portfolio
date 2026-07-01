using System;
using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Education : BaseEntity
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Grade { get; set; } = string.Empty;
}
