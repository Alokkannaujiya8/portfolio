using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.ContactMessages;

public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record GetContactMessagesQuery : IRequest<IEnumerable<ContactMessageDto>>;

public class GetContactMessagesQueryHandler : IRequestHandler<GetContactMessagesQuery, IEnumerable<ContactMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContactMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContactMessageDto>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.Repository<ContactMessage>().GetAllAsync();
        return _mapper.Map<IEnumerable<ContactMessageDto>>(messages.OrderByDescending(m => m.CreatedAt));
    }
}

public record SubmitContactMessageCommand : IRequest<ContactMessageDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SubmitContactMessageCommandValidator : AbstractValidator<SubmitContactMessageCommand>
{
    public SubmitContactMessageCommandValidator()
    {
        RuleFor(m => m.Name).NotEmpty().MaximumLength(100);
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Message).NotEmpty().MaximumLength(1000);
    }
}

public class SubmitContactMessageCommandHandler : IRequestHandler<SubmitContactMessageCommand, ContactMessageDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubmitContactMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContactMessageDto> Handle(SubmitContactMessageCommand request, CancellationToken cancellationToken)
    {
        var msg = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            IsRead = false
        };

        await _unitOfWork.Repository<ContactMessage>().AddAsync(msg);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContactMessageDto>(msg);
    }
}

public record MarkMessageAsReadCommand(Guid Id) : IRequest<bool>;

public class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkMessageAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var msg = await _unitOfWork.Repository<ContactMessage>().GetByIdAsync(request.Id);
        if (msg == null) return false;

        msg.IsRead = true;
        msg.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ContactMessage>().Update(msg);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
