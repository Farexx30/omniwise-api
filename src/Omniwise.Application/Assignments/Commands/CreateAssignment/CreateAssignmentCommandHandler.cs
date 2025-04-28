using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Queries.GetAssignmentById;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IAssignmentsRepository assignmentsRepository,
    ICoursesRepository coursesRepository,
    ILogger<CreateAssignmentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<CreateAssignmentCommand, int>
{
    public async Task<int> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        var assignment = mapper.Map<Assignment>(request);

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to create {nameof(Assignment)} for course with id = {courseId}.");
        }

        var assignmentId = await assignmentsRepository.CreateAsync(assignment);

        return assignmentId;
    }
}
