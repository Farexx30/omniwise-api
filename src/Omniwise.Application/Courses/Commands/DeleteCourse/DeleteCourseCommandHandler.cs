using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler(ICoursesRepository coursesRepository,
        ILogger<DeleteCourseCommandHandler> logger,
        IUserContext userContext,
        IAuthorizationService authorizationService) : IRequestHandler<DeleteCourseCommand>
{
    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await coursesRepository.GetCourseByIdAsync(request.Id)
        ?? throw new NotFoundException($"Course with id = {request.Id} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, course, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to delete course with id = {request.Id}.");
        }

        logger.LogInformation("Course with id = {id} is deleting.", request.Id);

        await coursesRepository.DeleteAsync(course);
    }
}
