using AutoMapper;
using TaskManager.Desktop.Data.Entities;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Mapping;

public sealed class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskModel, TaskEntity>()
            .ReverseMap();
    }
}
