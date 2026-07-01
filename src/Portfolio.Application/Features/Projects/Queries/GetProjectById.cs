using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

public class GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
        return project == null ? null : mapper.Map<ProjectDto>(project);
    }
}
