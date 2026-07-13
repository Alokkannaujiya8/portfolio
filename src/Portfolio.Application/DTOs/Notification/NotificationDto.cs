using System;
using System.Linq;
using FluentValidation;

namespace Portfolio.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Icon { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateNotificationDto
{
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Success, Error, Warning, Info
    public string? RedirectUrl { get; set; }
    public string? Icon { get; set; }
}

public class UpdateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info";
    public string? RedirectUrl { get; set; }
    public string? Icon { get; set; }
    public bool IsRead { get; set; }
}

public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
{
    private static readonly string[] AllowedTypes = { "Success", "Error", "Warning", "Info" };

    public CreateNotificationDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(type => AllowedTypes.Contains(type))
            .WithMessage("Type must be one of: Success, Error, Warning, Info.");
    }
}

public class UpdateNotificationDtoValidator : AbstractValidator<UpdateNotificationDto>
{
    private static readonly string[] AllowedTypes = { "Success", "Error", "Warning", "Info" };

    public UpdateNotificationDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(type => AllowedTypes.Contains(type))
            .WithMessage("Type must be one of: Success, Error, Warning, Info.");
    }
}
