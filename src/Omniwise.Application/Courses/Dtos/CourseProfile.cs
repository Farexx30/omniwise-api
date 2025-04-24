using AutoMapper;
using Omniwise.Domain.Entities;

namespace Omniwise.Application.Courses.Dtos;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>();
    }
    
}
