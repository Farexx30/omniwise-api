using Omniwise.Application.Common.Interfaces.Repositories;
using Omniwise.Application.Common.Services.Notifications;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Exceptions;
using Quartz;

namespace Omniwise.Infrastructure.Jobs;

internal class CheckOverdueAssignmentJob(INotificationService notificationService,
    IAssignmentsRepository assignmentsRepository,
    IUserCourseRepository userCourseRepository) : IJob
{
    internal const string Name = nameof(CheckOverdueAssignmentJob);
    public async Task Execute(IJobExecutionContext context)
    {
        var assignmentId = context.MergedJobDataMap.GetInt("AssignmentId");
        var assignment = await assignmentsRepository.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException($"Assignment with id = {assignmentId} doesn't exist.");
        var courseId = assignment.CourseId;

        var courseMembers = await userCourseRepository.GetEnrolledCourseMembersWithRolesAsync(courseId);
        var assigmentSubmissionAuthorIds = await assignmentsRepository.GetAssigmentSubmissionAuthorIds(assignmentId);
        var overdueMembers = courseMembers
            .Where(member => !assigmentSubmissionAuthorIds.Contains(member.UserId))
            .ToList();

        var teacherIds = overdueMembers
            .Where(author => author.RoleName == Roles.Teacher)
            .Select(author => author.UserId)
            .ToList();

        var overdueAssignmentMemberIds = overdueMembers
            .Where(author => author.RoleName == Roles.Student)
            .Select(author => author.UserId)
            .ToList();

        if (overdueAssignmentMemberIds.Count == 0)
        {
            return;
        }

        var overdueAssignmentAuthorNames = courseMembers
            .Where(member => overdueAssignmentMemberIds.Contains(member.UserId))
            .Select(member => $"{member.FirstName} {member.LastName}")
            .ToList();

        var notificationContent = string.Empty;
        if (overdueAssignmentMemberIds.Count == 1)
            notificationContent = $"Assignment \"{assignment.Name}\" is overdue for 1 member: {string.Join(Environment.NewLine, overdueAssignmentAuthorNames)}";
        else
            notificationContent = $"Assignment \"{assignment.Name}\" is overdue for {overdueAssignmentMemberIds.Count} members:{Environment.NewLine}{string.Join(Environment.NewLine, overdueAssignmentAuthorNames)}";

        await notificationService.NotifyUsersAsync(notificationContent, teacherIds);
    }
}