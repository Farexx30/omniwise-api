using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetOwnedCourses;

public class GetOwnedCoursesQueryHandler(ILogger<GetOwnedCoursesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository) : IRequestHandler<GetOwnedCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetOwnedCoursesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all owned courses for user with id: {UserId} from the repository.", request.Id);
        var ownedCourses = await coursesRepository.GetAllOwnedCoursesAsync(request.Id);
        var ownedCoursesDtos = mapper.Map<IEnumerable<CourseDto>>(ownedCourses);

        return ownedCoursesDtos;
    }
}
