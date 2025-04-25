using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQueryHandler(ILogger<GetEnrolledCoursesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository) : IRequestHandler<GetEnrolledCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all enrolled courses for user with id: {UserId} from the repository.", request.Id);
        var enrolledCourses = await coursesRepository.GetAllEnrolledCoursesAsync(request.Id);
        var enrolledCoursesDtos = mapper.Map<IEnumerable<CourseDto>>(enrolledCourses);

        return enrolledCoursesDtos;
    }
}
