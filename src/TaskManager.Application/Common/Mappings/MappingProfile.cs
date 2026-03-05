using AutoMapper;
using TaskManager.Application.Common.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Sin categoria")
            );

        CreateMap<Category, CategoryDto>();
    }
}