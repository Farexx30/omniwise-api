using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces.Identity;
using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.CourseMembers.Dtos;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Queries.GetPendingCourseMembers;

public class GetPendingCourseMembersQueryHandler(ILogger<GetPendingCourseMembersQueryHandler> logger,
    IMapper mapper,
    IUserContext userContext,
    ICoursesRepository coursesRepository,
    IUserCourseRepository userCourseRepository,
    IAuthorizationService authorizationService) : IRequestHandler<GetPendingCourseMembersQuery, IEnumerable<PendingCourseMemberDto>>
{
    public async Task<IEnumerable<PendingCourseMemberDto>> Handle(GetPendingCourseMembersQuery request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;

        var isCourseExist = await coursesRepository.ExistsAsync(courseId);
        if (!isCourseExist)
        {
            logger.LogWarning("Course with id = {courseId} doesn't exist.", courseId);
            throw new NotFoundException($"Course doesn't exist.");
        }

        var authorizationCourseMember = new UserCourse { CourseId = courseId };
        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException($"You are not allowed to get pending course members for the course.");
        }

        logger.LogInformation("Fetching all pending course members for course with id: {CourseId} from the repository.", request.CourseId);
        
        var pendingCourseMembers = await userCourseRepository.GetPendingCourseMembersAsync(courseId);
        var pendingCourseMembersDtos = mapper.Map<IEnumerable<PendingCourseMemberDto>>(pendingCourseMembers);

        return pendingCourseMembersDtos;
    }
}
