using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler(ICoursesRepository coursesRepository,
        ILogger<UpdateCourseCommandHandler> logger,
        IMapper mapper,
        IUserContext userContext,
        IAuthorizationService authorizationService) : IRequestHandler<UpdateCourseCommand>
{
    public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await coursesRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException($"Course with id = {request.Id} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, course, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to update course with id = {request.Id}.");
        }

        logger.LogInformation("Course with id = {id} is updating.", request.Id);

        mapper.Map(request, course);
        await coursesRepository.SaveChangesAsync();
    }
}
