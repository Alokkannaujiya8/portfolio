using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Experiences;

public class ExperienceDto
{
    public Guid Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Description { get; set; } = string.Empty;
}

public record GetExperiencesQuery : IRequest<IEnumerable<ExperienceDto>>;

public class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, IEnumerable<ExperienceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExperiencesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
    {
        var experiences = await _unitOfWork.Repository<Experience>().GetAllAsync();
        return _mapper.Map<IEnumerable<ExperienceDto>>(experiences.OrderByDescending(e => e.StartDate));
    }
}

public record CreateExperienceCommand : IRequest<ExperienceDto>
{
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateExperienceCommandValidator : AbstractValidator<CreateExperienceCommand>
{
    public CreateExperienceCommandValidator()
    {
        RuleFor(e => e.Company).NotEmpty().MaximumLength(150);
        RuleFor(e => e.Role).NotEmpty().MaximumLength(150);
        RuleFor(e => e.StartDate).NotEmpty();
    }
}

public class CreateExperienceCommandHandler : IRequestHandler<CreateExperienceCommand, ExperienceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateExperienceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExperienceDto> Handle(CreateExperienceCommand request, CancellationToken cancellationToken)
    {
        var experience = new Experience
        {
            Company = request.Company,
            Role = request.Role,
            Location = request.Location,
            StartDate = request.StartDate,
            EndDate = request.IsCurrent ? null : request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description
        };

        await _unitOfWork.Repository<Experience>().AddAsync(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExperienceDto>(experience);
    }
}

public record UpdateExperienceCommand : IRequest<ExperienceDto>
{
    public Guid Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class UpdateExperienceCommandValidator : AbstractValidator<UpdateExperienceCommand>
{
    public UpdateExperienceCommandValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Company).NotEmpty().MaximumLength(150);
        RuleFor(e => e.Role).NotEmpty().MaximumLength(150);
        RuleFor(e => e.StartDate).NotEmpty();
    }
}

public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, ExperienceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateExperienceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
    {
        var experience = await _unitOfWork.Repository<Experience>().GetByIdAsync(request.Id);
        if (experience == null)
            throw new KeyNotFoundException($"Experience with ID {request.Id} not found.");

        experience.Company = request.Company;
        experience.Role = request.Role;
        experience.Location = request.Location;
        experience.StartDate = request.StartDate;
        experience.EndDate = request.IsCurrent ? null : request.EndDate;
        experience.IsCurrent = request.IsCurrent;
        experience.Description = request.Description;
        experience.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Experience>().Update(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExperienceDto>(experience);
    }
}

public record DeleteExperienceCommand(Guid Id) : IRequest<bool>;

public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExperienceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
    {
        var experience = await _unitOfWork.Repository<Experience>().GetByIdAsync(request.Id);
        if (experience == null) return false;

        _unitOfWork.Repository<Experience>().Delete(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
