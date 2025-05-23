using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetAvailableToEnrollCourses;

public class GetAvailableToEnrollCoursesQueryHandler(ILogger<GetAvailableToEnrollCoursesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    IFileService fileService,
    IUserContext userContext) : IRequestHandler<GetAvailableToEnrollCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetAvailableToEnrollCoursesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Getting available to enroll courses");
        var availableCourses = await coursesRepository.GetAvailableToEnrollCoursesMatchingAsync(request.SearchPhrase, currentUser.Id!);

        List<string?> imgUrls = [];
        foreach (var course in availableCourses)
        {
            if (course.ImgBlobName is not null)
            {
                var imgSasUrl = await fileService.GetFileSasUrl(course.ImgBlobName);
                imgUrls.Add(imgSasUrl);
            }
            else
            {
                imgUrls.Add(default);
            }
        }

        var availableCoursesDtos = mapper.Map<List<CourseDto>>(availableCourses);

        for (int i = 0; i < availableCoursesDtos.Count; ++i)
        {
            var courseDto = availableCoursesDtos[i];
            courseDto.ImgUrl = imgUrls[i];
        }

        return availableCoursesDtos;
    }
}
