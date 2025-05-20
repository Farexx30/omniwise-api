using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Notifications;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Commands.CreateLecture;

public class CreateLectureCommandHandler(ILogger<CreateLectureCommandHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    ILecturesRepository lecturesRepository,
    IUserCourseRepository userCourseRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    INotificationService notificationService): IRequestHandler<CreateLectureCommand, int>
{
    public async Task<int> Handle(CreateLectureCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var courseId = request.CourseId;
        
        var course = await coursesRepository.GetCourseByIdAsync(courseId) 
            ?? throw new NotFoundException($"Course not found.");

        var lecture = mapper.Map<Lecture>(request);

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to create Lecture in course with id {courseId}.");
        }

        logger.LogInformation("Creating a new lecture {@request} by teacher with id: {currentUserId}",
            request,
            currentUser.Id);

        var lectureId = await lecturesRepository.CreateAsync(lecture);

        

        var courseMembers = await userCourseRepository.GetEnrolledCourseMembersAsync(courseId);
        var studentIds = courseMembers.Select(member => member.UserId).ToList();
        var teacherIds = await userCourseRepository.GetTeacherIdsAsync(courseId);
        studentIds.RemoveAll(id => teacherIds.Contains(id));

        var notificationContent = $"New lecture added in course {course.Name}.";
        await notificationService.NotifyUsersAsync(notificationContent, studentIds);



        return lectureId;
    }
}
