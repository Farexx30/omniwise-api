using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Commands.DeleteLecture;

public class DeleteLectureCommandHandler(ILogger<DeleteLectureCommandHandler> logger,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService) : IRequestHandler<DeleteLectureCommand>
{
    public async Task Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var lectureId = request.LectureId;

        var lecture = await lecturesRepository.GetByIdAsync(lectureId)
            ?? throw new NotFoundException($"Lecture with id = {lectureId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to delete lecture with id = {lectureId}.",
                currentUser.Id, 
                lectureId);

            throw new ForbiddenException($"You are not allowed to delete lecture with id = {lectureId}.");
        }

        logger.LogInformation("Lecture with id = {id} is deleting.", lectureId);

        var fileNamesToDelete = lecture.Files
            .Select(f => f.BlobName)
            .ToList();

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await fileService.DeleteAllAsync(fileNamesToDelete);

            await lecturesRepository.DeleteAsync(lecture);
        });
    }
}