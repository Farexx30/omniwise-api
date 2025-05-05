using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.UserCourses.Dtos;
using Omniwise.Application.UserCourses.Queries.GetEnrolledCourseMembers;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.UserCourses.Queries.GetEnrolledCourseMembers;

public class GetEnrolledCourseMembersQueryHandler(ILogger<GetEnrolledCourseMembersQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCourseRepository,
    IAuthorizationService authorizationService) : IRequestHandler<GetEnrolledCourseMembersQuery, IEnumerable<EnrolledUserCourseDto>>
{
    public async Task<IEnumerable<EnrolledUserCourseDto>> Handle(GetEnrolledCourseMembersQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var authorizationUserCourse = new UserCourse { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationUserCourse, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get enrolled course members for course with id = {courseId}.");
        }

        logger.LogInformation("Fetching all enrolled course members for course with id: {CourseId} from the repository.", request.CourseId);

        var enrolledCourseMembers = await userCourseRepository.GetEnrolledCourseMembersAsync(courseId);
        var enrolledCourseMembersDtos = mapper.Map<IEnumerable<EnrolledUserCourseDto>>(enrolledCourseMembers);

        return enrolledCourseMembersDtos;
    }
}