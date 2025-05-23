using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetOwnedCourses;

public class GetOwnedCoursesQueryHandler(ILogger<GetOwnedCoursesQueryHandler> logger,
    IMapper mapper,
    IFileService fileService,
    ICoursesRepository coursesRepository,
    IUserContext userContext) : IRequestHandler<GetOwnedCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetOwnedCoursesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Fetching all owned courses for user with id: {UserId} from the repository.", currentUser.Id);

        var ownedCourses = await coursesRepository.GetAllOwnedCoursesAsync(currentUser.Id!);

        List<string?> imgUrls = [];
        foreach (var course in ownedCourses)
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

        var ownedCoursesDtos = mapper.Map<List<CourseDto>>(ownedCourses);

        for (int i = 0; i < ownedCoursesDtos.Count; ++i)
        {
            var courseDto = ownedCoursesDtos[i];
            courseDto.ImgUrl = imgUrls[i];
        }

        return ownedCoursesDtos;
    }
}
