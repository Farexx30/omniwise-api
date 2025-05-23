using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.UpdateAssignmentSubmission;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Notifications;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.GradeAssignmentSubmission;

public class GradeAssignmentSubmissionCommandHandler(IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    IAssignmentsRepository assignmentsRepository,
    ILogger<GradeAssignmentSubmissionCommandHandler> logger,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    INotificationService notificationService) : IRequestHandler<GradeAssignmentSubmissionCommand>
{
    public async Task Handle(GradeAssignmentSubmissionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionId = request.AssignmentSubmissionId;

        var assignmentSubmission = await assignmentSubmissionsRepository.GetByIdAsync(assignmentSubmissionId)
            ?? throw new NotFoundException($"Assignment submission with id = {assignmentSubmissionId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to grade assignment submission with id = {assignmentSubmissionId}.",
                currentUser.Id,
                assignmentSubmissionId);

            throw new ForbiddenException($"You are not allowed to grade {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId}");
        }

        var grade = request.Grade;

        if (grade is not null)
        {
            var maxGrade = await assignmentsRepository.GetMaxGradeAsync(assignmentSubmission.AssignmentId);
            if (grade > maxGrade)
            {
                throw new BadRequestException($"Grade for this assignment cannot be greater than {maxGrade}.");
            }
        }
       
        //We don't use automapper here for better readability:
        assignmentSubmission.Grade = grade;

        await assignmentSubmissionsRepository.SaveChangesAsync();

        var notificationDetails = await assignmentSubmissionsRepository.GetRelatedAssignmentAndCourseNamesAsync(assignmentSubmissionId) 
            ?? throw new NotFoundException($"Assignment submission with id = {assignmentSubmissionId} not found.");

        var courseName = notificationDetails.CourseName;
        var assignmentName = notificationDetails.AssignmentName;
        var notificationContent = $"Assignment \"{assignmentName}\" in course \"{courseName}\" was graded. " +
            $"Grade: {assignmentSubmission.Grade}";

        await notificationService.NotifyUserAsync(notificationContent, assignmentSubmission.AuthorId);
    }
}
