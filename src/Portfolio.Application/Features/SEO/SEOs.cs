using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.SEO;

public class SEODto
{
    public Guid Id { get; set; }
    public string PageName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}

public record GetSEOQuery(string PageName) : IRequest<SEODto?>;

public class GetSEOQueryHandler : IRequestHandler<GetSEOQuery, SEODto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSEOQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SEODto?> Handle(GetSEOQuery request, CancellationToken cancellationToken)
    {
        var seos = await _unitOfWork.Repository<Domain.Entities.SEO>().FindAsync(s => s.PageName == request.PageName);
        var seo = seos.FirstOrDefault();
        return seo == null ? null : _mapper.Map<SEODto>(seo);
    }
}

public record GetAllSEOQuery : IRequest<IEnumerable<SEODto>>;

public class GetAllSEOQueryHandler : IRequestHandler<GetAllSEOQuery, IEnumerable<SEODto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSEOQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SEODto>> Handle(GetAllSEOQuery request, CancellationToken cancellationToken)
    {
        var seos = await _unitOfWork.Repository<Domain.Entities.SEO>().GetAllAsync();
        return _mapper.Map<IEnumerable<SEODto>>(seos);
    }
}

public record UpdateSEOCommand : IRequest<SEODto>
{
    public Guid Id { get; set; }
    public string PageName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}

public class UpdateSEOCommandValidator : AbstractValidator<UpdateSEOCommand>
{
    public UpdateSEOCommandValidator()
    {
        RuleFor(s => s.PageName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Title).NotEmpty().MaximumLength(200);
    }
}

public class UpdateSEOCommandHandler : IRequestHandler<UpdateSEOCommand, SEODto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSEOCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SEODto> Handle(UpdateSEOCommand request, CancellationToken cancellationToken)
    {
        var seo = await _unitOfWork.Repository<Domain.Entities.SEO>().GetByIdAsync(request.Id);
        if (seo == null)
        {
            // Try to find by PageName
            var existingSeos = await _unitOfWork.Repository<Domain.Entities.SEO>().FindAsync(s => s.PageName == request.PageName);
            seo = existingSeos.FirstOrDefault();
        }

        if (seo == null)
        {
            seo = new Domain.Entities.SEO
            {
                PageName = request.PageName,
                Title = request.Title,
                Description = request.Description,
                Keywords = request.Keywords
            };
            await _unitOfWork.Repository<Domain.Entities.SEO>().AddAsync(seo);
        }
        else
        {
            seo.Title = request.Title;
            seo.Description = request.Description;
            seo.Keywords = request.Keywords;
            seo.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Domain.Entities.SEO>().Update(seo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SEODto>(seo);
    }
}
