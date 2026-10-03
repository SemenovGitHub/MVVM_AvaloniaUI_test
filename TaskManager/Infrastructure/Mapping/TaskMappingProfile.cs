using AutoMapper;
using TaskManager.Domain.Models;
using TaskManager.Infrastructure.Persistence.Entities;
using TaskManager.Presentation.Contracts;

namespace TaskManager.Infrastructure.Mapping;

public sealed class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<CreateTaskRequest, TaskItem>()
            .ForMember(task => task.Id, options => options.Ignore())
            .ForMember(task => task.IsCompleted, options => options.Ignore())
            .ForMember(task => task.CreatedAt, options => options.Ignore())
            .ForMember(task => task.Title, options => options.MapFrom(request => (request.Title ?? string.Empty).Trim()));

        CreateMap<TaskItem, TaskItemEntity>()
            .ReverseMap();
        
        CreateMap<TaskItem, TaskResponse>();
    }
}
