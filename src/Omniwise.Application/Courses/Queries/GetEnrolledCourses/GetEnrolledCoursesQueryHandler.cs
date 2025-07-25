﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Files;
using Omniwise.Application.Courses.Dtos;

namespace Omniwise.Application.Courses.Queries.GetEnrolledCourses;

public class GetEnrolledCoursesQueryHandler(ILogger<GetEnrolledCoursesQueryHandler> logger,
    IMapper mapper,
    ICoursesRepository coursesRepository,
    IFileService fileService,
    IUserContext userContext) : IRequestHandler<GetEnrolledCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Fetching all enrolled courses for user with id: {UserId} from the repository.", currentUser.Id);

        var enrolledCourses = await coursesRepository.GetAllEnrolledCoursesMatchingAsync(request.SearchPhrase, currentUser.Id!);

        List<string?> imgUrls = [];
        foreach (var course in enrolledCourses)
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

        var enrolledCoursesDtos = mapper.Map<List<CourseDto>>(enrolledCourses);

        for (int i = 0; i < enrolledCoursesDtos.Count; ++i)
        {
            var courseDto = enrolledCoursesDtos[i];
            courseDto.ImgUrl = imgUrls[i];
        }

        return enrolledCoursesDtos;
    }
}
