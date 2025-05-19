namespace Omniwise.Application.Common.Interfaces;

public interface IQuartzSchedulerService
{
    Task ScheduleAssignmentCheckJob(int assignmentId, DateTime deadline);
    Task UpdateScheduledAssignmentCheckJob(int assignmentId, DateTime dateTime);
    Task DeleteScheduledAssignmentCheckJob(int assignmentId);
}
