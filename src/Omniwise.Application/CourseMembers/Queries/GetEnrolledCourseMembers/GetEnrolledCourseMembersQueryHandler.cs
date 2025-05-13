using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Queries.GetEnrolledCourseMembers;

public class GetEnrolledCourseMembersQueryHandler(ILogger<GetEnrolledCourseMembersQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCourseRepository,
    IAuthorizationService authorizationService) : IRequestHandler<GetEnrolledCourseMembersQuery, IEnumerable<EnrolledCourseMemberDto>>
{
    public async Task<IEnumerable<EnrolledCourseMemberDto>> Handle(GetEnrolledCourseMembersQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var authorizationCourseMember = new UserCourse { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get enrolled course members for course with id = {courseId}.");
        }

        logger.LogInformation("Fetching all enrolled course members for course with id: {CourseId} from the repository.", request.CourseId);

        var enrolledCourseMembers = await userCourseRepository.GetEnrolledCourseMembersAsync(courseId);
        var enrolledCourseMembersDtos = mapper.Map<IEnumerable<EnrolledCourseMemberDto>>(enrolledCourseMembers);

        return enrolledCourseMembersDtos;
    }
}