using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Files;
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
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService,
    INotificationService notificationService): IRequestHandler<CreateLectureCommand, int>
{
    public async Task<int> Handle(CreateLectureCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var courseId = request.CourseId;

        var course = await coursesRepository.GetCourseByIdAsync(courseId)
            ?? throw new NotFoundException($"Course with id = {courseId} not found.");

        var lecture = mapper.Map<Lecture>(request);

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to create lecture in course with id = {courseId}.",
                currentUser.Id,
                courseId);

            throw new ForbiddenException($"You are not allowed to create lecture in course with id = {courseId}.");
        }

        logger.LogInformation("Creating a new lecture {@request} by teacher with id: {currentUserId}",
            request,
            currentUser.Id);

        var files = request.Files;

        int lectureId = 0;
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            lectureId = await lecturesRepository.CreateAsync(lecture);

            var lectureFiles = await fileService.UploadAllAsync<LectureFile>(files, lectureId);
            lecture.Files.AddRange(lectureFiles);
            await lecturesRepository.SaveChangesAsync();
        });
     
        var courseMembers = await userCourseRepository.GetEnrolledCourseMembersAsync(courseId);
        var studentIds = courseMembers.Select(member => member.UserId).ToList();
        var teacherIds = await userCourseRepository.GetTeacherIdsAsync(courseId);
        studentIds.RemoveAll(teacherIds.Contains);

        var notificationContent = $"New lecture added in course {course.Name}.";
        await notificationService.NotifyUsersAsync(notificationContent, studentIds);

        return lectureId;
    }
}
