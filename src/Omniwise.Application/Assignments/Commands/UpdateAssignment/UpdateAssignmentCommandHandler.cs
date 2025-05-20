using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
using Omniwise.Application.Common.Interfaces;
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
    ICoursesRepository coursesRepository,
    ILogger<UpdateAssignmentCommandHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IQuartzSchedulerService quartzSchedulerService) : IRequestHandler<UpdateAssignmentCommand>
{
    public async Task Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignmentId = request.AssignmentId;
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} not found.");
        }

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} in {nameof(Course)} with id = {courseId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to update {nameof(Assignment)} with id = {assignmentId} in {nameof(Course)} with id = {courseId}.");
        }

        mapper.Map(request, assignment);

        await assignmentsRepository.SaveChangesAsync();

        await quartzSchedulerService.UpdateScheduledAssignmentCheckJob(assignmentId, assignment.Deadline);
    }
}
