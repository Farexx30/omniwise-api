using AutoMapper;
using Omniwise.Domain.Entities;
using Omniwise.Application.UserCourses.Commands.AddPendingCourseMember;

namespace Omniwise.Application.UserCourses.Dtos;

public class UserCourseMappingProfile : Profile
{
    public UserCourseMappingProfile()
    {
        CreateMap<AddPendingCourseMemberCommand, UserCourse>();
    }
}
