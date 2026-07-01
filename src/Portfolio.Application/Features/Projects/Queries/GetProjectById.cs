using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
        return project == null ? null : _mapper.Map<ProjectDto>(project);
    }
}
