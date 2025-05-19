using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Exceptions;
using Omniwise.Infrastructure.Jobs;
using Quartz;

namespace Omniwise.Infrastructure.Services;

public class QuartzSchedulerService(ISchedulerFactory schedulerFactory) : IQuartzSchedulerService
{
    private readonly IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

    public async Task ScheduleAssignmentCheckJob(int assignmentId, DateTime deadline)
    {
        var jobData = new JobDataMap
        {
            { "AssignmentId", assignmentId }
        };

        var job = JobBuilder.Create<CheckOverdueAssignmentJob>()
            .WithIdentity($"CheckAssignment-{assignmentId}")
            .UsingJobData(jobData)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"Trigger-Assignment-{assignmentId}")
            .StartAt(deadline.AddMinutes(1))
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task UpdateScheduledAssignmentCheckJob(int assignmentId, DateTime dateTime)
    {
        var triggerKey = new TriggerKey($"Trigger-Assignment-{assignmentId}");
        var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(dateTime)
                .Build();

        await _scheduler.RescheduleJob(triggerKey, trigger);
    }

    public async Task DeleteScheduledAssignmentCheckJob(int assignmentId)
    {
        var triggerKey = new TriggerKey($"Trigger-Assignment-{assignmentId}");
        await _scheduler.UnscheduleJob(triggerKey);
    }
}
