using AutoMapper;
using Portfolio.Domain.Entities;
using Portfolio.Application.Features.Projects;
using Portfolio.Application.Features.Skills;
using Portfolio.Application.Features.Experiences;
using Portfolio.Application.Features.Educations;
using Portfolio.Application.Features.Certificates;
using Portfolio.Application.Features.Services;
using Portfolio.Application.Features.Blogs;
using Portfolio.Application.Features.Gallery;
using Portfolio.Application.Features.Resumes;
using Portfolio.Application.Features.ContactMessages;
using Portfolio.Application.Features.Settings;
using Portfolio.Application.Features.SEO;
using Portfolio.Application.Features.Home;

namespace Portfolio.Application.Common.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Hero, HeroDto>().ReverseMap();
        CreateMap<About, AboutDto>().ReverseMap();
        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<Skill, SkillDto>().ReverseMap();
        CreateMap<Experience, ExperienceDto>().ReverseMap();
        CreateMap<Education, EducationDto>().ReverseMap();
        CreateMap<Certificate, CertificateDto>().ReverseMap();
        CreateMap<Service, ServiceDto>().ReverseMap();
        CreateMap<Blog, BlogDto>().ReverseMap();
        CreateMap<GalleryItem, GalleryItemDto>().ReverseMap();
        CreateMap<Resume, ResumeDto>().ReverseMap();
        CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();
        CreateMap<Setting, SettingDto>().ReverseMap();
        CreateMap<Domain.Entities.SEO, SEODto>().ReverseMap();
    }
}
