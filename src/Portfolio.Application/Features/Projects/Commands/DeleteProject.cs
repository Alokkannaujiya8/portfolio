using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Projects.Commands;

public record DeleteProjectCommand(Guid Id) : IRequest<bool>;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.Id);
        if (project == null)
        {
            return false;
        }

        _unitOfWork.Repository<Project>().Delete(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
