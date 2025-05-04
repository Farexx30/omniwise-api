using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.UserCourses.Commands.AddPendingCourseMember;

public class AddPendingCourseMemberCommandHandler(ILogger<AddPendingCourseMemberCommandHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCoursesRepository,
    IUserContext userContext) : IRequestHandler<AddPendingCourseMemberCommand>
{
    public async Task Handle(AddPendingCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var userId = userContext.GetCurrentUser().Id;
        request.UserId = userId!;

        var isCourseExisting = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExisting)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        var courseMember = mapper.Map<UserCourse>(request);

        var isCourseMemberExisting = await userCoursesRepository.ExistsAsync(courseId, userId!);
        if (isCourseMemberExisting)
        {
            logger.LogWarning("UserCourse relation for user {userId} and course {courseId} already exists.", userId, courseId);
            throw new ForbiddenException($"User with id = {userId} has already sent an enroll request to course with id = {courseId}.");
        }

        await userCoursesRepository.AddPendingCourseMemberAsync(courseMember);
    }
}
