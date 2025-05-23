using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Assignments.Commands.CreateAssignment;
using Omniwise.Application.Assignments.Dtos;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Assignments.Queries.GetAllCourseAssignments;

public class GetAllCourseAssignmentsQueryHandler(IAssignmentsRepository assignmentsRepository,
    ICoursesRepository coursesRepository,
    ILogger<GetAllCourseAssignmentsQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<GetAllCourseAssignmentsQuery, IEnumerable<BasicAssignmentDto>>
{
    public async Task<IEnumerable<BasicAssignmentDto>> Handle(GetAllCourseAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"{nameof(Course)} with id = {courseId} doesn't exist.");
        }

        //demoAssignment is only used for authorization check (because basically CourseId is the only thing that matters):
        var demoAssignment = new Assignment { CourseId = courseId };

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, demoAssignment, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get {nameof(Assignment)}s for {nameof(Course)} with id = {courseId}.");
        }

        var assignments = await assignmentsRepository.GetAllCourseAssignmentsAsync(courseId);         
        var assignmentDtos = mapper.Map<IEnumerable<BasicAssignmentDto>>(assignments);

        return assignmentDtos;
    }
}
