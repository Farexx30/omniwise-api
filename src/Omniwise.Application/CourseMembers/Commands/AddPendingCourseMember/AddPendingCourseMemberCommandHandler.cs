using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Notifications;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Commands.AddPendingCourseMember;

public class AddPendingCourseMemberCommandHandler(ILogger<AddPendingCourseMemberCommandHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCoursesRepository,
    IUserContext userContext,
    INotificationService notificationService) : IRequestHandler<AddPendingCourseMemberCommand>
{
    public async Task Handle(AddPendingCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetCurrentUser().Id;
        request.UserId = userId!;
         
        var course = await coursesRepository.GetCourseByIdAsync(request.CourseId) 
            ?? throw new NotFoundException($"Course not found.");

        var courseMember = mapper.Map<UserCourse>(request);

        var isCourseMemberExisting = await userCoursesRepository.ExistsAsync(course.Id, userId!);
        if (isCourseMemberExisting)
        {
            logger.LogWarning("UserCourse relation already exists.");
            throw new ForbiddenException($"User has already sent an enroll request to the course .");
        }

        await userCoursesRepository.AddCourseMemberAsync(courseMember);

        var notificationContent = $"There is a new enrollment request for the course {course.Name}";
        await notificationService.NotifyUserAsync(notificationContent, course.OwnerId);
    }
}
