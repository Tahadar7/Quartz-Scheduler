using Quartz;
using QuartzScheduler.API.Services;
using QuartzScheduler.Data.Entities;
using QuartzScheduler.Data.Repositories;
using System.Diagnostics;

namespace QuartzScheduler.API.Jobs;

[DisallowConcurrentExecution]
public class SendEmailJob(
    IJobRepository jobRepository,
    IJobHistoryRepository historyRepository,
    IEmailService emailService,
    ILogger<SendEmailJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var jobId = context.JobDetail.JobDataMap.GetInt("JobId");
        var stopwatch = Stopwatch.StartNew();

        var history = new JobExecutionHistory { JobId = jobId };

        try
        {
            var emailJob = await jobRepository.GetEmailJobByIdAsync(jobId, context.CancellationToken);

            if (emailJob is null)
            {
                // job was deleted between scheduling and firing nothing to run,
                // and nothing to record it against either
                logger.LogWarning("SendEmailJob fired for job {JobId} but it no longer exists.", jobId);
                return;
            }

            await emailService.SendEmailAsync(
                emailJob.ToAddress, emailJob.Subject, emailJob.Body, emailJob.IsHtml, context.CancellationToken);

            history.IsSuccess = true;
            emailJob.LastExecutedAt = DateTime.UtcNow;
            await jobRepository.UpdateAsync(emailJob, context.CancellationToken);
        }
        catch (Exception ex)
        {
            history.IsSuccess = false;
            history.ErrorMessage = ex.Message;
            logger.LogError(ex, "SendEmailJob failed for job {JobId}", jobId);
        }
        // gurantees a history row
        finally
        {
            stopwatch.Stop();
            history.DurationMs = stopwatch.ElapsedMilliseconds;
            history.ExecutedAt = DateTime.UtcNow;
            await historyRepository.AddAsync(history, context.CancellationToken);
        }
    }
}