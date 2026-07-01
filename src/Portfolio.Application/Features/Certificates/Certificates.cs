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

namespace Portfolio.Application.Features.Certificates;

public class CertificateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string CredentialId { get; set; } = string.Empty;
    public string CredentialUrl { get; set; } = string.Empty;
}

public record GetCertificatesQuery : IRequest<IEnumerable<CertificateDto>>;

public class GetCertificatesQueryHandler : IRequestHandler<GetCertificatesQuery, IEnumerable<CertificateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCertificatesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CertificateDto>> Handle(GetCertificatesQuery request, CancellationToken cancellationToken)
    {
        var certificates = await _unitOfWork.Repository<Certificate>().GetAllAsync();
        return _mapper.Map<IEnumerable<CertificateDto>>(certificates.OrderByDescending(c => c.IssueDate));
    }
}

public record CreateCertificateCommand : IRequest<CertificateDto>
{
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string CredentialId { get; set; } = string.Empty;
    public string CredentialUrl { get; set; } = string.Empty;
}

public class CreateCertificateCommandValidator : AbstractValidator<CreateCertificateCommand>
{
    public CreateCertificateCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(150);
        RuleFor(c => c.Issuer).NotEmpty().MaximumLength(150);
        RuleFor(c => c.IssueDate).NotEmpty();
    }
}

public class CreateCertificateCommandHandler : IRequestHandler<CreateCertificateCommand, CertificateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCertificateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CertificateDto> Handle(CreateCertificateCommand request, CancellationToken cancellationToken)
    {
        var certificate = new Certificate
        {
            Name = request.Name,
            Issuer = request.Issuer,
            IssueDate = request.IssueDate,
            ExpirationDate = request.ExpirationDate,
            CredentialId = request.CredentialId,
            CredentialUrl = request.CredentialUrl
        };

        await _unitOfWork.Repository<Certificate>().AddAsync(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CertificateDto>(certificate);
    }
}

public record UpdateCertificateCommand : IRequest<CertificateDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string CredentialId { get; set; } = string.Empty;
    public string CredentialUrl { get; set; } = string.Empty;
}

public class UpdateCertificateCommandValidator : AbstractValidator<UpdateCertificateCommand>
{
    public UpdateCertificateCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(150);
        RuleFor(c => c.Issuer).NotEmpty().MaximumLength(150);
        RuleFor(c => c.IssueDate).NotEmpty();
    }
}

public class UpdateCertificateCommandHandler : IRequestHandler<UpdateCertificateCommand, CertificateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCertificateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CertificateDto> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
    {
        var certificate = await _unitOfWork.Repository<Certificate>().GetByIdAsync(request.Id);
        if (certificate == null)
            throw new KeyNotFoundException($"Certificate with ID {request.Id} not found.");

        certificate.Name = request.Name;
        certificate.Issuer = request.Issuer;
        certificate.IssueDate = request.IssueDate;
        certificate.ExpirationDate = request.ExpirationDate;
        certificate.CredentialId = request.CredentialId;
        certificate.CredentialUrl = request.CredentialUrl;
        certificate.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Certificate>().Update(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CertificateDto>(certificate);
    }
}

public record DeleteCertificateCommand(Guid Id) : IRequest<bool>;

public class DeleteCertificateCommandHandler : IRequestHandler<DeleteCertificateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCertificateCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
    {
        var certificate = await _unitOfWork.Repository<Certificate>().GetByIdAsync(request.Id);
        if (certificate == null) return false;

        _unitOfWork.Repository<Certificate>().Delete(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
