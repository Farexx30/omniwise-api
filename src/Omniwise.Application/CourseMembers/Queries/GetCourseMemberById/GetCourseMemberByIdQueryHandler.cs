using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Queries.GetCourseMemberById;

public class GetCourseMemberByIdQueryHandler(ILogger<GetCourseMemberByIdQueryHandler> logger,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCourseRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<GetCourseMemberByIdQuery, CourseMemberDetailsDto>
{
    public async Task<CourseMemberDetailsDto> Handle(GetCourseMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var courseId = request.CourseId;
        var memberId = request.MemberId;


        var isCourseExist = coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist.Result)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course doesn't exist.");
        }

        var courseMemberDto = await userCourseRepository.GetByIdAsync(memberId, courseId, currentUser)
            ?? throw new NotFoundException($"Course member for course doesn't exist.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, courseMemberDto, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get course member.");
        }

        logger.LogInformation("Getting course member with id = {memberId} for course with id = {courseId}",
            memberId,
            courseId);

        return courseMemberDto;
    }
}