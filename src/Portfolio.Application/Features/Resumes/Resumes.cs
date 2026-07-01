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

namespace Portfolio.Application.Features.Resumes;

public class ResumeDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record GetActiveResumeQuery : IRequest<ResumeDto?>;

public class GetActiveResumeQueryHandler : IRequestHandler<GetActiveResumeQuery, ResumeDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveResumeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResumeDto?> Handle(GetActiveResumeQuery request, CancellationToken cancellationToken)
    {
        var resumes = await _unitOfWork.Repository<Resume>().FindAsync(r => r.IsActive);
        var activeResume = resumes.FirstOrDefault();
        return activeResume == null ? null : _mapper.Map<ResumeDto>(activeResume);
    }
}

public record GetResumesQuery : IRequest<IEnumerable<ResumeDto>>;

public class GetResumesQueryHandler : IRequestHandler<GetResumesQuery, IEnumerable<ResumeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetResumesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResumeDto>> Handle(GetResumesQuery request, CancellationToken cancellationToken)
    {
        var resumes = await _unitOfWork.Repository<Resume>().GetAllAsync();
        return _mapper.Map<IEnumerable<ResumeDto>>(resumes.OrderByDescending(r => r.CreatedAt));
    }
}

public record UploadResumeCommand(System.IO.Stream Stream, string FileName) : IRequest<ResumeDto>;

public class UploadResumeCommandHandler : IRequestHandler<UploadResumeCommand, ResumeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public UploadResumeCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ResumeDto> Handle(UploadResumeCommand request, CancellationToken cancellationToken)
    {
        // Save file physically
        var relativePath = await _fileStorageService.SaveFileAsync(request.Stream, request.FileName, "resume");

        // Deactivate other resumes
        var existingActiveResumes = await _unitOfWork.Repository<Resume>().FindAsync(r => r.IsActive);
        foreach (var r in existingActiveResumes)
        {
            r.IsActive = false;
            r.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Resume>().Update(r);
        }

        var resume = new Resume
        {
            FileName = request.FileName,
            FilePath = relativePath,
            IsActive = true
        };

        await _unitOfWork.Repository<Resume>().AddAsync(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ResumeDto>(resume);
    }
}

public record ActivateResumeCommand(Guid Id) : IRequest<bool>;

public class ActivateResumeCommandHandler : IRequestHandler<ActivateResumeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateResumeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ActivateResumeCommand request, CancellationToken cancellationToken)
    {
        var targetResume = await _unitOfWork.Repository<Resume>().GetByIdAsync(request.Id);
        if (targetResume == null) return false;

        var allResumes = await _unitOfWork.Repository<Resume>().GetAllAsync();
        foreach (var r in allResumes)
        {
            r.IsActive = (r.Id == request.Id);
            r.ModifiedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Resume>().Update(r);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record DeleteResumeCommand(Guid Id) : IRequest<bool>;

public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteResumeCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
    {
        var resume = await _unitOfWork.Repository<Resume>().GetByIdAsync(request.Id);
        if (resume == null) return false;

        // Delete physical file
        _fileStorageService.DeleteFile(resume.FilePath);

        _unitOfWork.Repository<Resume>().Delete(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
