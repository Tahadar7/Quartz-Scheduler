using Quartz;

namespace QuartzScheduler.API.Jobs;

[DisallowConcurrentExecution]
public class CallHttpApiJob(ILogger<CallHttpApiJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var jobId = context.JobDetail.JobDataMap.GetInt("JobId");
        logger.LogInformation("CallHttpApiJob fired for job {JobId}", jobId);
        return Task.CompletedTask;
    }
}