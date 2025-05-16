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
            logger.LogWarning("Course doesn't exist.");
            throw new NotFoundException($"Course doesn't exist.");
        }

        var courseMember = await userCourseRepository.GetCourseMemberAsync(courseId, memberId)
            ?? throw new NotFoundException($"Course member not found.");



        var authorizationCourseMember = new UserCourse { CourseId = courseId, UserId = userId! };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("You are not allowed to remove course member from course.");
            throw new ForbiddenException($"You are not allowed to remove course member.");
        }


        await userCourseRepository.DeleteAsync(courseMember);
    }
}
