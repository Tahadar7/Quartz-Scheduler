using Quartz;

namespace QuartzScheduler.API.Jobs;

[DisallowConcurrentExecution]
public class SendEmailJob(ILogger<SendEmailJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var jobId = context.JobDetail.JobDataMap.GetInt("JobId");
        logger.LogInformation("SendEmailJob fired for job {JobId}", jobId);
        return Task.CompletedTask;
    }
}