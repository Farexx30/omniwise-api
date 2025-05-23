using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Commands.UpdateAssignmentSubmission;

class UpdateAssignmentSubmissionCommandHandler(IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    ILogger<UpdateAssignmentSubmissionCommandHandler> logger,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IFileService fileService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAssignmentSubmissionCommand>
{
    public async Task Handle(UpdateAssignmentSubmissionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionId = request.AssignmentSubmissionId;

        var assignmentSubmission = await assignmentSubmissionsRepository.GetByIdAsync(assignmentSubmissionId)
            ?? throw new NotFoundException($"Assignment submission with id = {assignmentSubmissionId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to update assignment submission with id = {assignmentSubmissionId}.",
                currentUser.Id,
                assignmentSubmissionId);

            throw new ForbiddenException($"You are not allowed to update {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId}");
        }

        if (assignmentSubmission.Grade is not null)
        {
            throw new ForbiddenException($"You cannot update {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId} because it has already been graded.");
        }

        var newFiles = request.Files;
        var currentFiles = assignmentSubmission.Files;

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await fileService.CompareAndUpdateAsync(newFiles, currentFiles, assignmentSubmissionId); //This will internally modify the currentFiles List as follow with comparing and updating logic.

            assignmentSubmission.LatestSubmissionDate = DateTime.UtcNow;

            await assignmentSubmissionsRepository.SaveChangesAsync();
        });
    }
}
