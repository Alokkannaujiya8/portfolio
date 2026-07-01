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

namespace Portfolio.Application.Features.Skills;

public class SkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
}

public record GetSkillsQuery : IRequest<IEnumerable<SkillDto>>;

public class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, IEnumerable<SkillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSkillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var skills = await _unitOfWork.Repository<Skill>().GetAllAsync();
        return _mapper.Map<IEnumerable<SkillDto>>(skills.OrderBy(s => s.DisplayOrder));
    }
}

public record CreateSkillCommand : IRequest<SkillDto>
{
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillCommandValidator()
    {
        RuleFor(s => s.Name).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Proficiency).InclusiveBetween(0, 100);
    }
}

public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, SkillDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSkillCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SkillDto> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = new Skill
        {
            Name = request.Name,
            IconUrl = request.IconUrl,
            Category = request.Category,
            Proficiency = request.Proficiency,
            DisplayOrder = request.DisplayOrder
        };

        await _unitOfWork.Repository<Skill>().AddAsync(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SkillDto>(skill);
    }
}

public record UpdateSkillCommand : IRequest<SkillDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
{
    public UpdateSkillCommandValidator()
    {
        RuleFor(s => s.Id).NotEmpty();
        RuleFor(s => s.Name).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Proficiency).InclusiveBetween(0, 100);
    }
}

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, SkillDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSkillCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SkillDto> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(request.Id);
        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID {request.Id} not found.");

        skill.Name = request.Name;
        skill.IconUrl = request.IconUrl;
        skill.Category = request.Category;
        skill.Proficiency = request.Proficiency;
        skill.DisplayOrder = request.DisplayOrder;
        skill.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Skill>().Update(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SkillDto>(skill);
    }
}

public record DeleteSkillCommand(Guid Id) : IRequest<bool>;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(request.Id);
        if (skill == null) return false;

        _unitOfWork.Repository<Skill>().Delete(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
