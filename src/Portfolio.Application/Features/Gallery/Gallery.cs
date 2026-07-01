using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Gallery;

public class GalleryItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public record GetGalleryQuery : IRequest<IEnumerable<GalleryItemDto>>;

public class GetGalleryQueryHandler : IRequestHandler<GetGalleryQuery, IEnumerable<GalleryItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGalleryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GalleryItemDto>> Handle(GetGalleryQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<GalleryItem>().GetAllAsync();
        return _mapper.Map<IEnumerable<GalleryItemDto>>(items.OrderByDescending(i => i.CreatedAt));
    }
}

public record CreateGalleryItemCommand : IRequest<GalleryItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class CreateGalleryItemCommandValidator : AbstractValidator<CreateGalleryItemCommand>
{
    public CreateGalleryItemCommandValidator()
    {
        RuleFor(g => g.Title).NotEmpty().MaximumLength(150);
        RuleFor(g => g.ImageUrl).NotEmpty();
    }
}

public class CreateGalleryItemCommandHandler : IRequestHandler<CreateGalleryItemCommand, GalleryItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGalleryItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GalleryItemDto> Handle(CreateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var item = new GalleryItem
        {
            Title = request.Title,
            ImageUrl = request.ImageUrl,
            Category = request.Category
        };

        await _unitOfWork.Repository<GalleryItem>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GalleryItemDto>(item);
    }
}

public record DeleteGalleryItemCommand(Guid Id) : IRequest<bool>;

public class DeleteGalleryItemCommandHandler : IRequestHandler<DeleteGalleryItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGalleryItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<GalleryItem>().GetByIdAsync(request.Id);
        if (item == null) return false;

        _unitOfWork.Repository<GalleryItem>().Delete(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
