using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Commands.RemoveCourseMember;

public class RemoveCourseMemberCommandHandler(ILogger<RemoveCourseMemberCommandHandler> logger,
    ICoursesRepository courseRepository,
    IUserCourseRepository userCourseRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<RemoveCourseMemberCommand>
{
    public async Task Handle(RemoveCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var memberId = request.UserId;
        var userId = userContext.GetCurrentUser().Id;

        var isCourseExist = await courseRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var courseMember = await userCourseRepository.GetEnrolledCourseMemberAsync(courseId, memberId)
            ?? throw new NotFoundException($"Course member with id = {memberId} in course with id = {courseId} not found.");



        var authorizationCourseMember = new UserCourse { CourseId = courseId, UserId = userId! };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("You are not allowed to remove course member with id = {memberId} from course with id = {courseId}.", memberId, courseId);
            throw new ForbiddenException($"You are not allowed to remove course member with id = {memberId} from course with id = {courseId}.");
        }


        await userCourseRepository.DeleteAsync(courseMember);
    }
}
