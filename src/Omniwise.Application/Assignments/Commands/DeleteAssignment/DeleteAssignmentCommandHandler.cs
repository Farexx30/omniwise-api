using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.UpdateAssignment;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Types;
using Omniwise.Application.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.DeleteAssignment;

public class DeleteAssignmentCommandHandler(IAssignmentsRepository assignmentsRepository,
    IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    IFilesRepository filesRepository,
    ILogger<DeleteAssignmentCommandHandler> logger,
    IUserContext userContext,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService,
    IQuartzSchedulerService quartzSchedulerService) : IRequestHandler<DeleteAssignmentCommand>
{
    public async Task Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentId = request.AssignmentId;

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to delete assignment with id = {assignmentId}.",
                currentUser.Id,
                assignmentId);

            throw new ForbiddenException($"You are not allowed to delete {nameof(Assignment)} with id = {assignmentId}.");
        }
        
        var fileNamesToDelete = assignment.Files
            .Select(f => f.BlobName)
            .ToList();

        var relatedAssignmentSubmissionIds = await assignmentSubmissionsRepository.GetAllIdsByAssignmentIdAsync(assignmentId);
        var relatedAssignmentSubmissionFileNames = await filesRepository.GetAllBlobNamesByParentIdsAsync<AssignmentSubmissionFile>(relatedAssignmentSubmissionIds);
        fileNamesToDelete.AddRange(relatedAssignmentSubmissionFileNames);

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await fileService.DeleteAllAsync(fileNamesToDelete);

            await assignmentsRepository.DeleteAsync(assignment);
            await filesRepository.DeleteOrphansByBlobNamesAsync(relatedAssignmentSubmissionFileNames);
        });

        await quartzSchedulerService.DeleteScheduledAssignmentCheckJob(assignmentId);
    }
}
