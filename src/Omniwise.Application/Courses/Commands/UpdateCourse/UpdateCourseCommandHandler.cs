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
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService,
        IUserContext userContext,
        IAuthorizationService authorizationService) : IRequestHandler<UpdateCourseCommand>
{
    public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.Id;

        var course = await coursesRepository.GetCourseByIdAsync(courseId)
                ?? throw new NotFoundException($"Course with id = {courseId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, course, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to update course with id = {courseId}.");
        }

        logger.LogInformation("Course with id = {id} is updating.", courseId);

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            mapper.Map(request, course);

            var currentCourseImgBlobName = course.ImgBlobName;
            if (currentCourseImgBlobName is not null)
            {
                await blobStorageService.DeleteBlobAsync(currentCourseImgBlobName);
            }

            var courseImg = request.Img;
            if (courseImg is not null)
            {
                var blobName = $"{FileFolders.CourseImages}/{courseId}-{courseImg.FileName}";

                using var stream = courseImg.OpenReadStream();
                await blobStorageService.UploadBlobAsync(stream, blobName);

                course.ImgBlobName = blobName;
            }
            else
            {
                course.ImgBlobName = null;
            }

            await coursesRepository.SaveChangesAsync();
        });
    }
}
