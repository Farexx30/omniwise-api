using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Commands.CreateAssignmentSubmission;
using Omniwise.Application.AssignmentSubmissions.Dtos;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.AssignmentSubmissions.Queries.GetAssignmentSubmissionById;

public class GetAssignmentSubmissionByIdQueryHandler(IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    ILogger<GetAssignmentSubmissionByIdQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IFileService fileService) : IRequestHandler<GetAssignmentSubmissionByIdQuery, AssignmentSubmissionDto>
{
    public async Task<AssignmentSubmissionDto> Handle(GetAssignmentSubmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentSubmissionId = request.AssignmentSubmissionId;

        var assignmentSubmission = await assignmentSubmissionsRepository.GetByIdAsync(assignmentSubmissionId, includeFiles: true, includeComments: true)
            ?? throw new NotFoundException($"{nameof(AssignmentSubmission)} with id = {assignmentSubmissionId} not found.");

        var courseMemberAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.MustBeEnrolledInCourse);
        if (!courseMemberAuthorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not authorized to get assignment submission with id = {assignmentSubmissionId}.",
                currentUser.Id,
                assignmentSubmissionId);

            throw new ForbiddenException($"You are not allowed to get {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId}");
        }
        
        //If user is a student he can only access his own submissions:
        var isStudent = currentUser.IsInRole(Roles.Student);
        if (isStudent)
        {
            var isAuthorAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignmentSubmission, Policies.SameOwner);
            if (!isAuthorAuthorizationResult.Succeeded)
            {
                logger.LogWarning("User with id = {userId} is not authorized to get assignment submission with id = {assignmentSubmissionId}.",
                    currentUser.Id,
                    assignmentSubmissionId);

                throw new ForbiddenException($"You are not allowed to get {nameof(AssignmentSubmission)} with id = {assignmentSubmissionId}");
            }
        }

        var assignmentSubmissionDto = mapper.Map<AssignmentSubmissionDto>(assignmentSubmission);
        foreach (var file in assignmentSubmission.Files)
        {
            var fileSasUrl = await fileService.GetFileSasUrl(file.BlobName);
            assignmentSubmissionDto.FileUrls.Add(fileSasUrl);
        }

        return assignmentSubmissionDto;
    }
}
