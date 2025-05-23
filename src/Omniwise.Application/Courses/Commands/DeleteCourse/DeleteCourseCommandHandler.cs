using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Courses.Commands.CreateCourse;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler(ICoursesRepository coursesRepository,
        ILecturesRepository lecturesRepository,
        IAssignmentsRepository assignmentsRepository,
        IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
        ILogger<DeleteCourseCommandHandler> logger,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IFileService fileService,
        IFilesRepository filesRepository,
        IAuthorizationService authorizationService) : IRequestHandler<DeleteCourseCommand>
{
    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.Id;

        var course = await coursesRepository.GetCourseByIdAsync(courseId)
        ?? throw new NotFoundException($"Course with id = {request.Id} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, course, Policies.SameOwner);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to delete course with id = {courseId}.");
        }

        logger.LogInformation("Course with id = {id} is deleting.", courseId);

        List<string> fileNamesToDelete = [];

        if (course.ImgBlobName is not null)
        {
            fileNamesToDelete.Add(course.ImgBlobName);
        }

        var relatedLectureIds = await lecturesRepository.GetAllIdsByCourseIdAsync(courseId);
        var relatedLectureFileNames = await filesRepository.GetAllBlobNamesByParentIdsAsync<LectureFile>(relatedLectureIds);

        var relatedAssignmentIds = await assignmentsRepository.GetAllIdsByCourseIdAsync(courseId);
        var relatedAssignmentFileNames = await filesRepository.GetAllBlobNamesByParentIdsAsync<AssignmentFile>(relatedAssignmentIds);

        var relatedAssignmentSubmissionIds = await assignmentSubmissionsRepository.GetAllIdsByAssignmentIdsAsync(relatedAssignmentIds);
        var relatedAssignmentSubmissionFileNames = await filesRepository.GetAllBlobNamesByParentIdsAsync<AssignmentSubmissionFile>(relatedAssignmentSubmissionIds);

        var relatedEntitiesFileNames = relatedLectureFileNames
            .Concat(relatedAssignmentFileNames)
            .Concat(relatedAssignmentSubmissionFileNames);

        fileNamesToDelete.AddRange(relatedEntitiesFileNames);

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await fileService.DeleteAllAsync(fileNamesToDelete);

            await coursesRepository.DeleteAsync(course);
            await filesRepository.DeleteOrphansByBlobNamesAsync(relatedEntitiesFileNames);
        });
    }
}
