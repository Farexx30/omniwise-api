using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.AssignmentSubmissions.Dtos;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Lectures.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Queries.GetLectureById;

public class GetLectureByIdQueryHandler(ILogger<GetLectureByIdQueryHandler> logger,
    IMapper mapper,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IFileService fileService,
    IAuthorizationService authorizationService) : IRequestHandler<GetLectureByIdQuery, LectureDto>
{
    public async Task<LectureDto> Handle(GetLectureByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var lectureId = request.LectureId;

        var lecture = await lecturesRepository.GetByIdAsync(lectureId)
            ?? throw new NotFoundException($"Lecture with id = {lectureId} not found.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User with id = {userId} is not allowed to get Lecture with id = {lectureId}.",
                currentUser.Id, 
                lectureId);

            throw new ForbiddenException($"You are not allowed to get Lecture with id = {lectureId}.");
        }

        logger.LogInformation("Getting lecture with id = {lectureId}.", lectureId);

        var lectureDto = mapper.Map<LectureDto>(lecture);
        foreach (var file in lecture.Files)
        {
            var fileSasUrl = await fileService.GetFileSasUrl(file.BlobName);
            lectureDto.FileUrls.Add(fileSasUrl);
        }

        return lectureDto;
    }
}
