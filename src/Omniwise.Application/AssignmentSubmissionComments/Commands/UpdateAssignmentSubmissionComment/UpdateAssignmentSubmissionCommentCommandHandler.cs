using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissionComments.Commands.CreateAssignmentSubmissionComment;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissionComments.Commands.UpdateAssignmentSubmissionComment;

public class UpdateAssignmentSubmissionCommentCommandHandler(IAssignmentSubmissionCommentsRepository assignmentSubmissionCommentsRepository,
    ILogger<UpdateAssignmentSubmissionCommentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateAssignmentSubmissionCommentCommand>
{
    public async Task Handle(UpdateAssignmentSubmissionCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionCommentId = request.AssignmentSubmissionCommentId;

        var assignmentSubmissionComment = await assignmentSubmissionCommentsRepository.GetByIdAsync(assignmentSubmissionCommentId)
            ?? throw new NotFoundException($"Assignment submission comment with id = {assignmentSubmissionCommentId} not found.");

        var isAuthorAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmissionComment, Policies.SameOwner);
        if (!isAuthorAuthorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to update assignment submission comment with id = {assignmentSubmissionCommentId}.",
                currentUser.Id,
                assignmentSubmissionCommentId);

            throw new ForbiddenException($"You are not allowed to update assignment submission comment with id = {assignmentSubmissionCommentId}.");
        }

        mapper.Map(request, assignmentSubmissionComment);

        await assignmentSubmissionCommentsRepository.SaveChangesAsync();
    }
}
