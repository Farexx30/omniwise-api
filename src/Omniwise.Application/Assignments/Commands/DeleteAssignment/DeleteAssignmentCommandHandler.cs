using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.UpdateAssignment;
using Omniwise.Application.Common.Interfaces;
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
    ICoursesRepository coursesRepository,
    ILogger<UpdateAssignmentCommandHandler> logger,
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IQuartzSchedulerService quartzSchedulerService) : IRequestHandler<DeleteAssignmentCommand>
{
    public async Task Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignmentId = request.AssignmentId;
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException($"{nameof(Assignment)} with id = {assignmentId} in {nameof(Course)} with id = {courseId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, assignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("You are not allowed to delete assignment with id = {assignmentId} in course with id = {courseId}.", 
                assignmentId, 
                courseId);

            throw new ForbiddenException($"You are not allowed to delete {nameof(Assignment)} with id = {assignmentId} in {nameof(Course)} with id = {courseId}.");
        }

        await assignmentsRepository.DeleteAsync(assignment);
        await quartzSchedulerService.DeleteScheduledAssignmentCheckJob(assignmentId);
    }
}
