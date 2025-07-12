using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Commands.UpdateLecture;

public class UpdateLectureCommandHandler(ILogger<UpdateLectureCommandHandler> logger,
    IMapper mapper,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateLectureCommand>
{
    public async Task Handle(UpdateLectureCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var lectureId = request.Id;

        var lecture = await lecturesRepository.GetByIdAsync(lectureId)
            ?? throw new NotFoundException($"Lecture with id = {lectureId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to update Lecture with id = {lectureId}.",
                currentUser.Id, 
                lectureId);

            throw new ForbiddenException($"You are not allowed to update Lecture with id {lectureId}.");
        }

        logger.LogInformation("Lecture with id = {id} is updating.", lectureId);

        var files = request.Files;

        await unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            mapper.Map(request, lecture);
            lecture.Name = lecture.Name.Trim();
            lecture.Content = lecture.Content?.Trim();

            await fileService.CompareAndUpdateAsync(files, lecture.Files, lectureId);

            await lecturesRepository.SaveChangesAsync();
        });
    }
}
    