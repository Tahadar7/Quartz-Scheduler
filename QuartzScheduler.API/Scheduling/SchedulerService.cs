using Quartz;
using QuartzScheduler.API.Jobs;
using QuartzScheduler.Data.Entities;
using QuartzScheduler.Data.Repositories;
using QuartzScheduler.Shared.Enums;

namespace QuartzScheduler.API.Scheduling;

public class SchedulerService(
    ISchedulerFactory schedulerFactory,
    IJobRepository jobRepository) : ISchedulerService
{
    public async Task ScheduleAsync(Job job, CancellationToken ct = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);

        var jobDetail = JobBuilder.Create(ResolveQuartzJobType(job.JobType))
            .WithIdentity(GetJobKey(job))
            .UsingJobData("JobId", job.Id)   // only the id execution fetches fresh data
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(GetTriggerKey(job))
            .WithCronSchedule(job.CronExpression)
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger, ct);
    }

    public async Task RescheduleAsync(Job job, CancellationToken ct = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);
        var triggerKey = GetTriggerKey(job);

        if (!await scheduler.CheckExists(triggerKey, ct))
        {
            // if it is inactive
            await ScheduleAsync(job, ct);
            return;
        }

        var newTrigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithCronSchedule(job.CronExpression)
            .Build();

        await scheduler.RescheduleJob(triggerKey, newTrigger, ct);
    }

    public async Task UnscheduleAsync(Job job, CancellationToken ct = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);
        await scheduler.DeleteJob(GetJobKey(job), ct);
    }

    public async Task PauseAsync(Job job, CancellationToken ct = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);
        await scheduler.PauseJob(GetJobKey(job), ct);
    }

    public async Task ResumeAsync(Job job, CancellationToken ct = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);
        await scheduler.ResumeJob(GetJobKey(job), ct);
    }

    public async Task RescheduleAllActiveJobsAsync(CancellationToken ct = default)
    {
        var activeJobs = await jobRepository.GetActiveJobsAsync(ct);

        foreach (var job in activeJobs)
        {
            await ScheduleAsync(job, ct);
        }
    }

    // picks the Quartz job class based on our JobType
    private static Type ResolveQuartzJobType(JobType jobType) => jobType switch
    {
        JobType.Email => typeof(SendEmailJob),
        JobType.Http => typeof(CallHttpApiJob),
        _ => throw new InvalidOperationException($"No Quartz job registered for job type '{jobType}'")
    };

    private static JobKey GetJobKey(Job job)
    {
        return new(job.Id.ToString(), job.JobType.ToString());
    }

    private static TriggerKey GetTriggerKey(Job job)
    {
        return new($"{job.Id}-trigger", job.JobType.ToString());
    }
}