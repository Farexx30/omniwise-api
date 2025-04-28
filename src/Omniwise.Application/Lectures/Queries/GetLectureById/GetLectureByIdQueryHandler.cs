using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Lectures.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Queries.GetLectureById;

public class GetLectureByIdQueryHandler(ILogger<GetLectureByIdQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<GetLectureByIdQuery, LectureDto>
{
    public async Task<LectureDto> Handle(GetLectureByIdQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var lectureId = request.LectureId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var lecture = await lecturesRepository.GetByIdAsync(courseId, lectureId)
            ?? throw new NotFoundException($"Lecture with id = {lectureId} for Course with id = {courseId} doesn't exist.");

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, lecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get Lecture with id = {lectureId}.");
        }

        logger.LogInformation("Getting lecture with id = {lectureId} for course with id = {courseId}",
            lectureId,
            courseId);

        var lectureDto = mapper.Map<LectureDto>(lecture);
        return lectureDto;
    }
}
