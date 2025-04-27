using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Commands.DeleteLecture;

public class DeleteLectureCommandHandler(ILogger<DeleteLectureCommandHandler> logger,
    ICoursesRepository coursesRepository,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<DeleteLectureCommand>
{
    public async Task Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var lectureId = request.Id;

        var isCourseExisting = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExisting)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} not found.");
        }

        var lecture = await lecturesRepository.GetByIdAsync(request.CourseId, request.Id)
            ?? throw new NotFoundException($"Lecture with id = {request.Id} for Course with id = {request.CourseId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to delete lecture with id = {request.Id}.");
        }

        logger.LogInformation("Lecture with id = {id} is deleting.", request.Id);

        await lecturesRepository.DeleteAsync(lecture);
    }
}