using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Blogs;

public class BlogDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record GetBlogsQuery(bool OnlyPublished = false) : IRequest<IEnumerable<BlogDto>>;

public class GetBlogsQueryHandler : IRequestHandler<GetBlogsQuery, IEnumerable<BlogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogDto>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
    {
        var blogs = await _unitOfWork.Repository<Blog>().GetAllAsync();
        if (request.OnlyPublished)
        {
            blogs = blogs.Where(b => b.IsPublished);
        }
        return _mapper.Map<IEnumerable<BlogDto>>(blogs.OrderByDescending(b => b.PublishedAt ?? b.CreatedAt));
    }
}

public record GetBlogBySlugQuery(string Slug) : IRequest<BlogDto?>;

public class GetBlogBySlugQueryHandler : IRequestHandler<GetBlogBySlugQuery, BlogDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlogBySlugQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BlogDto?> Handle(GetBlogBySlugQuery request, CancellationToken cancellationToken)
    {
        var blogs = await _unitOfWork.Repository<Blog>().FindAsync(b => b.Slug == request.Slug);
        var blog = blogs.FirstOrDefault();
        return blog == null ? null : _mapper.Map<BlogDto>(blog);
    }
}

public record CreateBlogCommand : IRequest<BlogDto>
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}

public class CreateBlogCommandValidator : AbstractValidator<CreateBlogCommand>
{
    public CreateBlogCommandValidator()
    {
        RuleFor(b => b.Title).NotEmpty().MaximumLength(200);
        RuleFor(b => b.Content).NotEmpty();
    }
}

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, BlogDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBlogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BlogDto> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Title.ToLower()
            .Replace(" ", "-")
            .Replace("?", "")
            .Replace("/", "")
            .Replace("&", "and"); // Basic slugification

        var blog = new Blog
        {
            Title = request.Title,
            Slug = slug,
            Summary = request.Summary,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTime.UtcNow : null
        };

        await _unitOfWork.Repository<Blog>().AddAsync(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BlogDto>(blog);
    }
}

public record UpdateBlogCommand : IRequest<BlogDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}

public class UpdateBlogCommandValidator : AbstractValidator<UpdateBlogCommand>
{
    public UpdateBlogCommandValidator()
    {
        RuleFor(b => b.Id).NotEmpty();
        RuleFor(b => b.Title).NotEmpty().MaximumLength(200);
        RuleFor(b => b.Content).NotEmpty();
    }
}

public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, BlogDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBlogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BlogDto> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _unitOfWork.Repository<Blog>().GetByIdAsync(request.Id);
        if (blog == null)
            throw new KeyNotFoundException($"Blog with ID {request.Id} not found.");

        var slug = request.Title.ToLower()
            .Replace(" ", "-")
            .Replace("?", "")
            .Replace("/", "")
            .Replace("&", "and");

        blog.Title = request.Title;
        blog.Slug = slug;
        blog.Summary = request.Summary;
        blog.Content = request.Content;
        blog.ImageUrl = request.ImageUrl;
        
        if (request.IsPublished && !blog.IsPublished)
        {
            blog.PublishedAt = DateTime.UtcNow;
        }
        else if (!request.IsPublished)
        {
            blog.PublishedAt = null;
        }

        blog.IsPublished = request.IsPublished;
        blog.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Blog>().Update(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BlogDto>(blog);
    }
}

public record DeleteBlogCommand(Guid Id) : IRequest<bool>;

public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBlogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _unitOfWork.Repository<Blog>().GetByIdAsync(request.Id);
        if (blog == null) return false;

        _unitOfWork.Repository<Blog>().Delete(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
