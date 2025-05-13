using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Commands.AcceptCourseMember;

public class AcceptCourseMemberCommandHandler(ILogger<AcceptCourseMemberCommandHandler> logger,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCoursesRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<AcceptCourseMemberCommand>
{
    public async Task Handle(AcceptCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExisting = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExisting)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        var authorizationCourseMember = new UserCourse { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to accept course member for course with id = {courseId}.");
        }

        var pendingCourseMember = await userCoursesRepository.GetPendingCourseMemberAsync(courseId, request.UserId)
            ?? throw new NotFoundException($"Pending course member with id = {request.UserId} not found.");

        logger.LogInformation("Accepting course member with id = {userId} for course with id = {courseId}.", request.UserId, courseId);

        pendingCourseMember.IsAccepted = true;
        pendingCourseMember.JoinDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await userCoursesRepository.SaveChangesAsync();

    }
}
