using AutoMapper;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Application.Courses.Commands.UpdateCourse;
using Omniwise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Dtos;

public class CoursesMappingProfile : Profile
{
    public CoursesMappingProfile()
    {
        CreateMap<CreateCourseCommand, Course>();
        CreateMap<UpdateCourseCommand, Course>();
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.ImgName,
                opt => opt.MapFrom(src => GetImgOriginalName(src.ImgBlobName)));
    }

    private static string? GetImgOriginalName(string? imgBlobName)
    {
        if (imgBlobName is null)
        {
            return null;
        }

        var withoutPrefix = imgBlobName["course-images/".Length..];
        int resourceIdDashIndex = withoutPrefix.IndexOf('-');
        return withoutPrefix[(resourceIdDashIndex + 1)..];
    }
}
