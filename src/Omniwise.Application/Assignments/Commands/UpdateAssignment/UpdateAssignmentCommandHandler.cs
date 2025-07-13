using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
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

namespace Omniwise.Application.Assignments.Commands.UpdateAssignment;

public class UpdateAssignmentCommandHandler(IAssignmentsRepository assignmentsRepository,
    ILogger<UpdateAssignmentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService,
    IQuartzSchedulerService quartzSchedulerService) : IRequestHandler<UpdateAssignmentCommand>
{
    public async Task Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentId = request.AssignmentId;

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId, includeFiles: true)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to update assignment with id = {assignmentId}.", 
                currentUser.Id, 
                assignmentId);

            throw new ForbiddenException($"You are not allowed to update {nameof(Assignment)} with id = {assignmentId}.");
        }

        var files = request.Files;

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            mapper.Map(request, assignment);
            assignment.Name = assignment.Name.Trim();
            assignment.Content = assignment.Content?.Trim();

            await fileService.CompareAndUpdateAsync(files, assignment.Files, assignmentId);

            await assignmentsRepository.SaveChangesAsync();
        });

        await quartzSchedulerService.UpdateScheduledAssignmentCheckJob(assignmentId, assignment.Deadline);
    }
}
