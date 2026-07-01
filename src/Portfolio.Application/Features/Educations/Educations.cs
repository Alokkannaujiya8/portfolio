using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Educations;

public class EducationDto
{
    public Guid Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public record GetEducationsQuery : IRequest<IEnumerable<EducationDto>>;

public class GetEducationsQueryHandler : IRequestHandler<GetEducationsQuery, IEnumerable<EducationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEducationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EducationDto>> Handle(GetEducationsQuery request, CancellationToken cancellationToken)
    {
        var educations = await _unitOfWork.Repository<Education>().GetAllAsync();
        return _mapper.Map<IEnumerable<EducationDto>>(educations.OrderByDescending(e => e.StartDate));
    }
}

public record CreateEducationCommand : IRequest<EducationDto>
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public class CreateEducationCommandValidator : AbstractValidator<CreateEducationCommand>
{
    public CreateEducationCommandValidator()
    {
        RuleFor(e => e.Institution).NotEmpty().MaximumLength(150);
        RuleFor(e => e.Degree).NotEmpty().MaximumLength(150);
        RuleFor(e => e.StartDate).NotEmpty();
    }
}

public class CreateEducationCommandHandler : IRequestHandler<CreateEducationCommand, EducationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEducationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EducationDto> Handle(CreateEducationCommand request, CancellationToken cancellationToken)
    {
        var education = new Education
        {
            Institution = request.Institution,
            Degree = request.Degree,
            FieldOfStudy = request.FieldOfStudy,
            StartDate = request.StartDate,
            EndDate = request.IsCurrent ? null : request.EndDate,
            IsCurrent = request.IsCurrent,
            Grade = request.Grade
        };

        await _unitOfWork.Repository<Education>().AddAsync(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EducationDto>(education);
    }
}

public record UpdateEducationCommand : IRequest<EducationDto>
{
    public Guid Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public class UpdateEducationCommandValidator : AbstractValidator<UpdateEducationCommand>
{
    public UpdateEducationCommandValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Institution).NotEmpty().MaximumLength(150);
        RuleFor(e => e.Degree).NotEmpty().MaximumLength(150);
        RuleFor(e => e.StartDate).NotEmpty();
    }
}

public class UpdateEducationCommandHandler : IRequestHandler<UpdateEducationCommand, EducationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEducationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EducationDto> Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
    {
        var education = await _unitOfWork.Repository<Education>().GetByIdAsync(request.Id);
        if (education == null)
            throw new KeyNotFoundException($"Education with ID {request.Id} not found.");

        education.Institution = request.Institution;
        education.Degree = request.Degree;
        education.FieldOfStudy = request.FieldOfStudy;
        education.StartDate = request.StartDate;
        education.EndDate = request.IsCurrent ? null : request.EndDate;
        education.IsCurrent = request.IsCurrent;
        education.Grade = request.Grade;
        education.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Education>().Update(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EducationDto>(education);
    }
}

public record DeleteEducationCommand(Guid Id) : IRequest<bool>;

public class DeleteEducationCommandHandler : IRequestHandler<DeleteEducationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEducationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        var education = await _unitOfWork.Repository<Education>().GetByIdAsync(request.Id);
        if (education == null) return false;

        _unitOfWork.Repository<Education>().Delete(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
