using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Commands.UpdateLecture;

public class UpdateLectureCommandHandler(ILogger<UpdateLectureCommandHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateLectureCommand>
{
    public async Task Handle(UpdateLectureCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var lectureId = request.Id;

        var isCourseExisting = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExisting)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var lecture = await lecturesRepository.GetByIdAsync(courseId, lectureId)
            ?? throw new NotFoundException($"Lecture with id = {lectureId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to update Lecture with id {lectureId}.");
        }

        logger.LogInformation("Lecture with id = {id} is updating.", request.Id);

        var files = request.Files;

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            mapper.Map(request, lecture);

            await fileService.CompareAndUpdateAsync(files, lecture.Files, lectureId);

            await lecturesRepository.SaveChangesAsync();
        });
    }
}
    