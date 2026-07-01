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

namespace Portfolio.Application.Features.Home;

public class HeroDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = string.Empty;
    public string SecondaryButtonText { get; set; } = string.Empty;
}

public class AboutDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int ProjectsCompleted { get; set; }
}

public record GetHeroQuery : IRequest<HeroDto?>;

public class GetHeroQueryHandler : IRequestHandler<GetHeroQuery, HeroDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetHeroQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HeroDto?> Handle(GetHeroQuery request, CancellationToken cancellationToken)
    {
        var heroes = await _unitOfWork.Repository<Hero>().GetAllAsync();
        var hero = heroes.FirstOrDefault();
        return hero == null ? null : _mapper.Map<HeroDto>(hero);
    }
}

public record GetAboutQuery : IRequest<AboutDto?>;

public class GetAboutQueryHandler : IRequestHandler<GetAboutQuery, AboutDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAboutQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AboutDto?> Handle(GetAboutQuery request, CancellationToken cancellationToken)
    {
        var abouts = await _unitOfWork.Repository<About>().GetAllAsync();
        var about = abouts.FirstOrDefault();
        return about == null ? null : _mapper.Map<AboutDto>(about);
    }
}

public record UpdateHeroCommand : IRequest<HeroDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = string.Empty;
    public string SecondaryButtonText { get; set; } = string.Empty;
}

public class UpdateHeroCommandValidator : AbstractValidator<UpdateHeroCommand>
{
    public UpdateHeroCommandValidator()
    {
        RuleFor(h => h.Title).NotEmpty().MaximumLength(200);
        RuleFor(h => h.Subtitle).NotEmpty();
    }
}

public class UpdateHeroCommandHandler : IRequestHandler<UpdateHeroCommand, HeroDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateHeroCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HeroDto> Handle(UpdateHeroCommand request, CancellationToken cancellationToken)
    {
        var hero = await _unitOfWork.Repository<Hero>().GetByIdAsync(request.Id);
        if (hero == null)
        {
            var existingHeroes = await _unitOfWork.Repository<Hero>().GetAllAsync();
            hero = existingHeroes.FirstOrDefault();
        }

        if (hero == null)
        {
            hero = new Hero
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                ImageUrl = request.ImageUrl,
                ResumeUrl = request.ResumeUrl,
                PrimaryButtonText = request.PrimaryButtonText,
                SecondaryButtonText = request.SecondaryButtonText
            };
            await _unitOfWork.Repository<Hero>().AddAsync(hero);
        }
        else
        {
            hero.Title = request.Title;
            hero.Subtitle = request.Subtitle;
            hero.ImageUrl = request.ImageUrl;
            hero.ResumeUrl = request.ResumeUrl;
            hero.PrimaryButtonText = request.PrimaryButtonText;
            hero.SecondaryButtonText = request.SecondaryButtonText;
            hero.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Hero>().Update(hero);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<HeroDto>(hero);
    }
}

public record UpdateAboutCommand : IRequest<AboutDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int ProjectsCompleted { get; set; }
}

public class UpdateAboutCommandValidator : AbstractValidator<UpdateAboutCommand>
{
    public UpdateAboutCommandValidator()
    {
        RuleFor(a => a.Title).NotEmpty().MaximumLength(200);
        RuleFor(a => a.Description).NotEmpty();
    }
}

public class UpdateAboutCommandHandler : IRequestHandler<UpdateAboutCommand, AboutDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAboutCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AboutDto> Handle(UpdateAboutCommand request, CancellationToken cancellationToken)
    {
        var about = await _unitOfWork.Repository<About>().GetByIdAsync(request.Id);
        if (about == null)
        {
            var existingAbouts = await _unitOfWork.Repository<About>().GetAllAsync();
            about = existingAbouts.FirstOrDefault();
        }

        if (about == null)
        {
            about = new About
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Location = request.Location,
                Email = request.Email,
                Phone = request.Phone,
                ExperienceYears = request.ExperienceYears,
                ProjectsCompleted = request.ProjectsCompleted
            };
            await _unitOfWork.Repository<About>().AddAsync(about);
        }
        else
        {
            about.Title = request.Title;
            about.Subtitle = request.Subtitle;
            about.Description = request.Description;
            about.ImageUrl = request.ImageUrl;
            about.Location = request.Location;
            about.Email = request.Email;
            about.Phone = request.Phone;
            about.ExperienceYears = request.ExperienceYears;
            about.ProjectsCompleted = request.ProjectsCompleted;
            about.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<About>().Update(about);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AboutDto>(about);
    }
}
