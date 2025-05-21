using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Application.Services.Files;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Exceptions;

namespace Omniwise.Application.CourseMembers.Commands.RemoveCourseMember;

public class RemoveCourseMemberCommandHandler(ILogger<RemoveCourseMemberCommandHandler> logger,
    ICoursesRepository courseRepository,
    IUserCourseRepository userCourseRepository,
    IAssignmentSubmissionsRepository assignmentSubmissionsRepository,
    IFilesRepository filesRepository,
    IFileService fileService,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService) : IRequestHandler<RemoveCourseMemberCommand>
{
    public async Task Handle(RemoveCourseMemberCommand request, CancellationToken cancellationToken)
    {
        var courseId = request.CourseId;
        var memberId = request.UserId;
        var userId = userContext.GetCurrentUser().Id!;

        var course = await courseRepository.GetCourseByIdAsync(courseId)
            ?? throw new NotFoundException($"Course doesn't exist.");

        var courseMember = await userCourseRepository.GetCourseMemberWithRoleNameAsync(courseId, memberId)
            ?? throw new NotFoundException($"Course member not found.");

        if (courseMember.RoleName == Roles.Teacher)
        {
            var isOwnerAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, course, Policies.SameOwner);
            if (!isOwnerAuthorizationResult.Succeeded)
            {
                logger.LogWarning("You are not allowed to remove this member from course.");

                throw new ForbiddenException($"You are not allowed to remove this member from course.");
            }

            await userCourseRepository.DeleteByUserIdAsync(courseMember.UserId);
        }

        if (courseMember.RoleName == Roles.Student)
        {
            var authorizationCourseMember = new UserCourse { CourseId = courseId, UserId = userId! };

            var isInCourseAuthorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, authorizationCourseMember, Policies.MustBeEnrolledInCourse);
            if (!isInCourseAuthorizationResult.Succeeded)
            {
                logger.LogWarning("You are not allowed to remove this member from course.");

                throw new ForbiddenException($"You are not allowed to remove this member from course.");
            }

            var relatedWithStudentAssignmentSubmissionIds = await assignmentSubmissionsRepository.GetAllIdsByAuthorIdAsync(memberId);
            var relatedWithStudentAssignmentSubmissionFileNames = await filesRepository.GetAllBlobNamesByParentIdsAsync<AssignmentSubmissionFile>(relatedWithStudentAssignmentSubmissionIds);

            await unitOfWork.ExecuteTransactionalAsync(async () =>
            {
                await fileService.DeleteAllAsync(relatedWithStudentAssignmentSubmissionFileNames);
                await filesRepository.DeleteOrphansByBlobNamesAsync(relatedWithStudentAssignmentSubmissionFileNames);

                await assignmentSubmissionsRepository.DeleteByAuthorIdAsync(courseMember.UserId);

                await userCourseRepository.DeleteByUserIdAsync(courseMember.UserId);
            });
        }      
    }
}
