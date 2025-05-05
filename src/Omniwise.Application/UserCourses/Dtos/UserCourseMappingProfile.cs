using AutoMapper;
using Omniwise.Domain.Entities;
using Omniwise.Application.UserCourses.Commands.AddPendingCourseMember;

namespace Omniwise.Application.UserCourses.Dtos;

public class UserCourseMappingProfile : Profile
{
    public UserCourseMappingProfile()
    {
        CreateMap<AddPendingCourseMemberCommand, UserCourse>();
        CreateMap<UserCourse, PendingUserCourseDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
        CreateMap<UserCourse, EnrolledUserCourseDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
    }
}
