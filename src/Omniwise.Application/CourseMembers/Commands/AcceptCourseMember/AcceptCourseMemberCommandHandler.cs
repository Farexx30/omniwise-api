using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Notifications;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Commands.AcceptCourseMember;

public class AcceptCourseMemberCommandHandler(ILogger<AcceptCourseMemberCommandHandler> logger,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCoursesRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    INotificationService notificationService) : IRequestHandler<AcceptCourseMemberCommand>
{
    public async Task Handle(AcceptCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;


        //change to get course by id
        var course = await coursesRepository.GetCourseByIdAsync(courseId) ??
            throw new NotFoundException($"Course not found.");

        var authorizationCourseMember = new UserCourse { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to accept course member for course.");
        }

        var pendingCourseMember = await userCoursesRepository.GetPendingCourseMemberAsync(courseId, request.UserId)
            ?? throw new NotFoundException($"Pending course member not found.");

        logger.LogInformation("Accepting course member for course {courseName}.", course.Name);

        pendingCourseMember.IsAccepted = true;
        pendingCourseMember.JoinDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await userCoursesRepository.SaveChangesAsync();

        var notificationContent = $"You have been accepted to course {course.Name}."; 
        await notificationService.NotifyUserAsync(notificationContent, request.UserId);
    }
}
