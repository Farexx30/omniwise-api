using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Courses.Dtos;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler(ILogger<GetCourseByIdQueryHandler> logger,
    IMapper mapper,
    IFileService fileService,
    ICoursesRepository coursesRepository) : IRequestHandler<GetCourseByIdQuery, CourseDto?>
{
    public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching course with ID {CourseId} from the repository.", request.Id);

        var course = await coursesRepository.GetCourseByIdAsync(request.Id)
            ?? throw new NotFoundException($"Course with ID {request.Id} doesn't exist");

        var courseDto = mapper.Map<CourseDto>(course);

        if (course.ImgBlobName is not null)
        {
            var imgSasUrl = await fileService.GetFileSasUrl(course.ImgBlobName);
            courseDto.ImgUrl = imgSasUrl;
        }

        return courseDto;
    }
}