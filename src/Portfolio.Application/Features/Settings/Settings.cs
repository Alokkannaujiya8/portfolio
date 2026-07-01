using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Features.Settings;

public class SettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public record GetSettingsQuery : IRequest<IEnumerable<SettingDto>>;

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, IEnumerable<SettingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSettingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SettingDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _unitOfWork.Repository<Setting>().GetAllAsync();
        return _mapper.Map<IEnumerable<SettingDto>>(settings);
    }
}

public record UpdateSettingsCommand(Dictionary<string, string> Settings) : IRequest<IEnumerable<SettingDto>>;

public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, IEnumerable<SettingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSettingsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SettingDto>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var existingSettings = (await _unitOfWork.Repository<Setting>().GetAllAsync()).ToList();

        foreach (var item in request.Settings)
        {
            var dbSetting = existingSettings.FirstOrDefault(s => s.Key.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
            if (dbSetting != null)
            {
                dbSetting.Value = item.Value;
                dbSetting.ModifiedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Setting>().Update(dbSetting);
            }
            else
            {
                var newSetting = new Setting { Key = item.Key, Value = item.Value };
                await _unitOfWork.Repository<Setting>().AddAsync(newSetting);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updatedSettings = await _unitOfWork.Repository<Setting>().GetAllAsync();
        return _mapper.Map<IEnumerable<SettingDto>>(updatedSettings);
    }
}
