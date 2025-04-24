using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Courses.Dtos;
using Omniwise.Domain.Exceptions;
using Omniwise.Domain.Repositories;

namespace Omniwise.Application.Courses.Queries;

public class GetCourseByIdQueryHandler(ILogger<GetCourseByIdQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository) : IRequestHandler<GetCourseByIdQuery, CourseDto?>
{
    public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching course with ID {CourseId} from the repository.", request.Id);
        var course = await coursesRepository.GetCourseByIdAsync(request.Id)
            ?? throw new NotFoundException();
        var courseDto = mapper.Map<CourseDto>(course);

        return courseDto;
    }
}