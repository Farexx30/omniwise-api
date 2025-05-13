using AutoMapper;
using Omniwise.Domain.Entities;
using Omniwise.Application.CourseMembers.Commands.AddPendingCourseMember;

namespace Omniwise.Application.CourseMembers.Dtos;

public class CourseMemberMappingProfile : Profile
{
    public CourseMemberMappingProfile()
    {
        CreateMap<AddPendingCourseMemberCommand, UserCourse>();
        CreateMap<UserCourse, PendingCourseMemberDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
        CreateMap<UserCourse, EnrolledCourseMemberDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
    }
}
