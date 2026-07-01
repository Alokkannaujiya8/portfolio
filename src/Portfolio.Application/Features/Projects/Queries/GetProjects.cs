using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Projects.Queries;

public record GetProjectsQuery : IRequest<IEnumerable<ProjectDto>>;

public class GetProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await unitOfWork.Repository<Project>().GetAllAsync();
        return mapper.Map<IEnumerable<ProjectDto>>(projects.OrderBy(p => p.DisplayOrder));
    }
}
