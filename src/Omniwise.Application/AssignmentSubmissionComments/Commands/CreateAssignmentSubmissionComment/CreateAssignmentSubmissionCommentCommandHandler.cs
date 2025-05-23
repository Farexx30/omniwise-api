using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;
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

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.CreateAssignmentSubmissionComment;

public class CreateAssignmentSubmissionCommentCommandHandler(IAssignmentSubmissionCommentsRepository assignmentSubmissionCommentsRepository,
    IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    IUserCourseRepository userCourseRepository,
    ILogger<CreateAssignmentSubmissionCommentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    INotificationService notificationService) : IRequestHandler<CreateAssignmentSubmissionCommentCommand, int>
{
    public async Task<int> Handle(CreateAssignmentSubmissionCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionId = request.AssignmentSubmissionId;

        var assignmentSubmission = await assignmentSubmissionsRepository.GetByIdAsync(assignmentSubmissionId)
            ?? throw new NotFoundException($"Assignment submission with id = {assignmentSubmissionId} not found.");

        var assignmentSubmissionComment = mapper.Map<AssignmentSubmissionComment>(request);
        assignmentSubmissionComment.SentDate = DateTime.UtcNow;
        assignmentSubmissionComment.AuthorId = currentUser.Id!;

        var isTeacher = currentUser.IsInRole(Roles.Teacher);
        if (isTeacher)
        {
            var courseMemeberAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmissionComment, Policies.MustBeEnrolledInCourse);
            if (!courseMemeberAuthorizationResult.Succeeded)
            {
                logger.LogWarning("User {UserId} is not allowed to create a comment on assignment submission {AssignmentSubmissionId}.", 
                    currentUser.Id, 
                    assignmentSubmissionId);

                throw new ForbiddenException($"You are not allowed to create a comment on assignment submission with id = {assignmentSubmissionId}.");
            }
        }

        var isStudent = currentUser.IsInRole(Roles.Student);
        if (isStudent)
        {
            var isAuthorAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.SameOwner);
            if (!isAuthorAuthorizationResult.Succeeded)
            {
                logger.LogWarning("User with id = {userId} is not authorized to get assignment submission with id = {assignmentSubmissionId}.",
                    currentUser.Id,
                    assignmentSubmissionId);

                throw new ForbiddenException($"You are not allowed to create a comment on assignment submission with id = {assignmentSubmissionId}.");
            }
        }

        var assignmentSubmissionCommentId = await assignmentSubmissionCommentsRepository.CreateAsync(assignmentSubmissionComment);

        var notificationDetails = await assignmentSubmissionCommentsRepository.GetDetailsToCommentNotificationAsync(assignmentSubmissionId);

        if (notificationDetails is not null)
        {
            notificationDetails.CommentAuthorFirstName = currentUser.FirstName!;
            notificationDetails.CommentAuthorLastName = currentUser.LastName!;

            string authorSubmissionName = $"{notificationDetails.AssignmentSubmissionAuthorFirstName} {notificationDetails.AssignmentSubmissionAuthorLastName}";
            string authorCommentName = $"{notificationDetails.CommentAuthorFirstName} {notificationDetails.CommentAuthorLastName}";
            var notificationContent = $"{authorCommentName} posted a new comment on {authorSubmissionName}'s assignment submission for {notificationDetails.AssignmentName} in {notificationDetails.CourseName}.{Environment.NewLine}Comment content:{Environment.NewLine}{assignmentSubmissionComment.Content}";
        
            var notificationReceivers = await userCourseRepository.GetTeacherIdsAsync(notificationDetails.CourseId);

            if (assignmentSubmission.AuthorId != assignmentSubmissionComment.AuthorId)
            {
                notificationReceivers.Add(assignmentSubmission.AuthorId);
                notificationReceivers.Remove(assignmentSubmissionComment.AuthorId);
            }

            await notificationService.NotifyUsersAsync(notificationContent, notificationReceivers);
        }

        return assignmentSubmissionCommentId;
    }
}
