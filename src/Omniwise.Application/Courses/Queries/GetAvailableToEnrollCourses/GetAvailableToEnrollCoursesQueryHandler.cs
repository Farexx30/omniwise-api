using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Courses.Dtos;
using Omniwise.Domain.Entities;

namespace Omniwise.Application.Courses.Queries.GetAvailableToEnrollCourses;

public class GetAvailableToEnrollCoursesQueryHandler(ILogger<GetAvailableToEnrollCoursesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    IUserContext userContext) : IRequestHandler<GetAvailableToEnrollCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetAvailableToEnrollCoursesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Getting available to enroll courses");
        var availableCourses = await coursesRepository.GetAvailableToEnrollCoursesMatchingAsync(request.SearchPhrase, currentUser.Id!);

        var availableCoursesDtos = mapper.Map<IEnumerable<CourseDto>>(availableCourses);

        return availableCoursesDtos;
    }
}
