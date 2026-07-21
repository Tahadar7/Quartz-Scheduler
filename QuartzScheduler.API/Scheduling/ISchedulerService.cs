using QuartzScheduler.Data.Entities;

namespace QuartzScheduler.API.Scheduling;
public interface ISchedulerService
{
    Task ScheduleAsync(Job job, CancellationToken ct = default);
    Task RescheduleAsync(Job job, CancellationToken ct = default);   // cron changed
    Task UnscheduleAsync(Job job, CancellationToken ct = default);
    Task PauseAsync(Job job, CancellationToken ct = default);
    Task ResumeAsync(Job job, CancellationToken ct = default);

    // called once at startup re-registers every active job with Quartz
    Task RescheduleAllActiveJobsAsync(CancellationToken ct = default);
}