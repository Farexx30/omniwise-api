﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
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

namespace Omniwise.Application.AssignmentSubmissions.Commands.DeleteAssignmentSubmission;

public class DeleteAssignmentSubmissionCommandHandler(IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    ILogger<DeleteAssignmentSubmissionCommandHandler> logger,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IFileService fileService,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteAssignmentSubmissionCommand>
{
    public async Task Handle(DeleteAssignmentSubmissionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionId = request.AssignmentSubmissionId;

        var assignmentSubmission = await assignmentSubmissionsRepository.GetByIdAsync(assignmentSubmissionId, includeFiles: true, includeAssignmentInfo: true)
            ?? throw new NotFoundException($"Assignment submission with id = {assignmentSubmissionId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to delete assignment submission with id = {assignmentSubmissionId}.",
                currentUser.Id,
                assignmentSubmissionId);

            throw new ForbiddenException($"You are not allowed to delete {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId}");
        }

        var hasDeadlinePassed = assignmentSubmission.Assignment.Deadline < DateTime.UtcNow;
        if (hasDeadlinePassed)
        {
            throw new ForbiddenException("You can't delete submission after deadline.");
        }

        if (assignmentSubmission.Grade is not null)
        {
            throw new ForbiddenException($"You cannot delete {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId} because it has already been graded.");
        }

        var fileNamesToDelete = assignmentSubmission.Files
            .Select(f => f.BlobName)
            .ToList();

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await fileService.DeleteAllAsync(fileNamesToDelete);

            await assignmentSubmissionsRepository.DeleteAsync(assignmentSubmission);
        });
    }
}
