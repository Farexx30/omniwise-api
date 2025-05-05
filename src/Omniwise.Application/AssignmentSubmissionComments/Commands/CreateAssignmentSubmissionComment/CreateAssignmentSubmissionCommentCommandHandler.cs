using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Files;
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
    ILogger<CreateAssignmentSubmissionCommentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<CreateAssignmentSubmissionCommentCommand, int>
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

        return assignmentSubmissionCommentId;
    }
}
