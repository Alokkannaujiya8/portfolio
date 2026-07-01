using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Projects.Commands;

public record UpdateProjectCommand : IRequest<ProjectDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ProjectUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
}

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Title).NotEmpty().MaximumLength(150);
        RuleFor(v => v.Description).NotEmpty();
    }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
        if (project == null)
        {
            throw new System.Collections.Generic.KeyNotFoundException($"Project with ID {request.Id} was not found.");
        }

        project.Title = request.Title;
        project.Description = request.Description;
        project.ImageUrl = request.ImageUrl;
        project.ProjectUrl = request.ProjectUrl;
        project.GithubUrl = request.GithubUrl;
        project.Technologies = request.Technologies;
        project.DisplayOrder = request.DisplayOrder;
        project.IsFeatured = request.IsFeatured;
        project.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Project>().Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }
}
