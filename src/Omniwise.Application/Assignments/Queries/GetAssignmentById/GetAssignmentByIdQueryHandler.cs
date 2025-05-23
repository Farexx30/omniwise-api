using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Common.Types;
using Omniwise.Application.Lectures.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Queries.GetAssignmentById;

public class GetAssignmentByIdQueryHandler(IAssignmentsRepository assignmentsRepository,
    ILogger<GetAssignmentByIdQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IFileService fileService,
    IAuthorizationService authorizationService) : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>
{
    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var assignmentId = request.AssignmentId;

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId, currentUser: currentUser, includeAssignmentSubmissions: true)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to get assignment with id = {assignmentId}.",
                currentUser.Id,
                assignmentId);

            throw new ForbiddenException($"You are not allowed to get {nameof(Assignment)} with id = {assignmentId}.");
        }

        logger.LogInformation("Getting assignment with id = {assignmentId}.", assignmentId);

        var assignmentDto = mapper.Map<AssignmentDto>(assignment);
        foreach (var file in assignment.Files)
        {
            var fileSasUrl = await fileService.GetFileSasUrl(file.BlobName);
            assignmentDto.FileUrls.Add(fileSasUrl);
        }

        return assignmentDto;
    }
}
