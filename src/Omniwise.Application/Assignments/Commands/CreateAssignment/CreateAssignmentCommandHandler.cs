using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Queries.GetAssignmentById;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Common.Services.Notifications;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IAssignmentsRepository assignmentsRepository,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCourseRepository,
    IMapper mapper,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService,
    INotificationService notificationService,
    IQuartzSchedulerService quartzSchedulerService) : IRequestHandler<CreateAssignmentCommand, int>
{
    public async Task<int> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var course = await coursesRepository.GetCourseByIdAsync(courseId)
            ?? throw new NotFoundException($"Course not found.");

        var assignment = mapper.Map<Assignment>(request);

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to create {nameof(Assignment)} for course with id = {courseId}.");
        }

        var files = request.Files;

        int assignmentId = 0;
        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            assignmentId = await assignmentsRepository.CreateAsync(assignment);

            var assignmentFiles = await fileService.UploadAllAsync<AssignmentFile>(files, assignmentId);
            assignment.Files.AddRange(assignmentFiles);
            await assignmentsRepository.SaveChangesAsync();
        });

        var courseMembers = await userCourseRepository.GetEnrolledCourseMembersAsync(courseId);
        var studentIds = courseMembers.Select(member => member.UserId).ToList();
        var teacherIds = await userCourseRepository.GetTeacherIdsAsync(courseId);
        studentIds.RemoveAll(teacherIds.Contains);

        var notificationContent = $"New assignment added in course {course.Name}.";

        await notificationService.NotifyUsersAsync(notificationContent, studentIds);
        await quartzSchedulerService.ScheduleAssignmentCheckJob(assignmentId, assignment.Deadline);

        return assignmentId;
    }
}
