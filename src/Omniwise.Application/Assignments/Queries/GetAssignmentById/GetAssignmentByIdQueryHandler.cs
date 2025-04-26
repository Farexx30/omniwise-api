using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Common.Interfaces;
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
    ICoursesRepository coursesRepository,
    ILogger<GetAssignmentByIdQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>
{
    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignmentId = request.AssignmentId;
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId, courseId)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} for {nameof(Course)} with id = {courseId} doesn't exist.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get {nameof(Assignment)} with id = {assignmentId}.");
        }

        logger.LogInformation("Getting assignment with id = {assignmentId} for course with id = {courseId}",
            assignmentId,
            courseId);

        var assignmentDto = mapper.Map<AssignmentDto>(assignment);

        return assignmentDto;
    }
}
