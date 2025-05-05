using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Lectures.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Lectures.Queries.GetLectures;

public class GetLecturesQueryHandler(ILogger<GetLecturesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    ILecturesRepository lecturesRepository,
    IUserContext userContext,
    IAuthorizationService authorizationService) : IRequestHandler<GetLecturesQuery, IEnumerable<LectureToGetAllDto>>
{
    public async Task<IEnumerable<LectureToGetAllDto>> Handle(GetLecturesQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExisting = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExisting)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course with id = {courseId} doesn't exist.");
        }

        var authorizationLecture = new Lecture { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationLecture, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get Lectures in course with id = {courseId}.");
        }

        logger.LogInformation("Fetching all lectures for course with id: {CourseId} from the repository.", courseId);

        var lectures = await lecturesRepository.GetAllLecturesAsync(courseId);
        var lecturesDtos = mapper.Map<IEnumerable<LectureToGetAllDto>>(lectures);


        return lecturesDtos;
    }
}
