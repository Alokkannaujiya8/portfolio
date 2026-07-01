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

namespace Portfolio.Application.Features.Services;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public record GetServicesQuery : IRequest<IEnumerable<ServiceDto>>;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, IEnumerable<ServiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetServicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _unitOfWork.Repository<Service>().GetAllAsync();
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }
}

public record CreateServiceCommand : IRequest<ServiceDto>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(s => s.Title).NotEmpty().MaximumLength(150);
        RuleFor(s => s.Description).NotEmpty();
    }
}

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateServiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new Service
        {
            Title = request.Title,
            Description = request.Description,
            Icon = request.Icon
        };

        await _unitOfWork.Repository<Service>().AddAsync(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ServiceDto>(service);
    }
}

public record UpdateServiceCommand : IRequest<ServiceDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(s => s.Id).NotEmpty();
        RuleFor(s => s.Title).NotEmpty().MaximumLength(150);
        RuleFor(s => s.Description).NotEmpty();
    }
}

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateServiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceDto> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
        if (service == null)
            throw new KeyNotFoundException($"Service with ID {request.Id} not found.");

        service.Title = request.Title;
        service.Description = request.Description;
        service.Icon = request.Icon;
        service.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Service>().Update(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ServiceDto>(service);
    }
}

public record DeleteServiceCommand(Guid Id) : IRequest<bool>;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteServiceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
        if (service == null) return false;

        _unitOfWork.Repository<Service>().Delete(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
