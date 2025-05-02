using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Static;
using Omniwise.Application.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;

public class CreateAssignmentSubmissionCommandHandler(IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    IAssignmentsRepository assignmentsRepository,
    IFilesRepository filesRepository,
    ILogger<CreateAssignmentSubmissionCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IFileService fileService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAssignmentSubmissionCommand, int>
{
    public async Task<int> Handle(CreateAssignmentSubmissionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentId = request.AssignmentId;

        var isAssignmentExist = await assignmentsRepository.ExistsAsync(assignmentId);
        if (!isAssignmentExist)
        {
            logger.LogWarning("Assignment with id = {assignmentId} doesn't exist.", assignmentId);
            throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} doesn't exist.");
        }

        //Prepare new assignment submission and authorize user if he is allowed to create it:
        var assignmentSubmission = mapper.Map<AssignmentSubmission>(request);
        assignmentSubmission.LatestSubmissionDate = DateTime.UtcNow;
        assignmentSubmission.AuthorId = currentUser.Id!;
        
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to create assignment submission for assignment with id = {assignmentId}.", 
                currentUser.Id, 
                assignmentId);

            throw new ForbiddenException($"You are not allowed to create {nameof(AssignmentSubmission)} for {nameof(Assignment)} with id = {assignmentId}");
        }

        var isAlreadySubmitted = await assignmentSubmissionsRepository.IsAlreadySubmitted(assignmentId, currentUser.Id!);
        if (isAlreadySubmitted)
        {
            throw new ForbiddenException("You can't create new submission for the same assignment again.");
        }

        //Validate files:
        var files = request.Files;
        fileService.ValidateFiles(files); //Will throw BadRequestException if validation fails

        //If everything is ok update database and upload files:
        int assignmentSubmissionId = 0;
        List<AssignmentSubmissionFile> assignmentSubmissionFiles = [];
        try
        {
            await unitOfWork.ExecuteTransactionalAsync(async () =>
            {
                assignmentSubmissionId = await assignmentSubmissionsRepository.CreateAsync(assignmentSubmission);

                foreach (var file in files)
                {
                    var assignmentSubmissionFile = await fileService.UploadFileAsync<AssignmentSubmissionFile>(file, assignmentSubmissionId);
                    assignmentSubmissionFile.AssignmentSubmissionId = assignmentSubmissionId;
                    assignmentSubmissionFiles.Add(assignmentSubmissionFile);
                }

                await filesRepository.CreateManyAsync(assignmentSubmissionFiles);
            });

            return assignmentSubmissionId;
        }
        catch (Exception)
        {
            await fileService.RollbackChangesAsync();

            throw new Exception("An unexpected error occurred while creating assignment submission.");
        }
    }
}
