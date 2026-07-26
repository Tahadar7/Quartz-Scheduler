using Quartz;
using QuartzScheduler.API.Services;
using QuartzScheduler.Data.Entities;
using QuartzScheduler.Data.Repositories;
using System.Diagnostics;

namespace QuartzScheduler.API.Jobs;

[DisallowConcurrentExecution]
public class CallHttpApiJob(
    IJobRepository jobRepository,
    IJobHistoryRepository historyRepository,
    IHttpApiService httpApiService,
    ILogger<CallHttpApiJob> logger) : IJob
{
    private const int MaxResponseSummaryLength = 2000;   // matches JobExecutionHistory.ResponseSummary max length

    public async Task Execute(IJobExecutionContext context)
    {
        var jobId = context.JobDetail.JobDataMap.GetInt("JobId");
        var stopwatch = Stopwatch.StartNew();

        var history = new JobExecutionHistory { JobId = jobId };

        try
        {
            var httpJob = await jobRepository.GetHttpJobByIdAsync(jobId, context.CancellationToken);

            if (httpJob is null)
            {
                logger.LogWarning("CallHttpApiJob fired for job {JobId} but it no longer exists.", jobId);
                return;
            }

            var result = await httpApiService.CallAsync(
                httpJob.Url,
                httpJob.HttpMethod.ToString(),
                httpJob.Headers,
                httpJob.RequestBody,
                context.CancellationToken);

            history.StatusCode = result.StatusCode;
            history.ResponseSummary = Truncate(result.ResponseBody, MaxResponseSummaryLength);

            // the call completed — a 4xx/5xx from the TARGET is still a successful
            history.IsSuccess = true;

            httpJob.LastExecutedAt = DateTime.UtcNow;
            await jobRepository.UpdateAsync(httpJob, context.CancellationToken);
        }
        catch (Exception ex)
        {
            // network-level failure: timeout, DNS failure, connection refused
            history.IsSuccess = false;
            history.ErrorMessage = ex.Message;
            logger.LogError(ex, "CallHttpApiJob failed for job {JobId}", jobId);
        }
        finally
        {
            stopwatch.Stop();
            history.DurationMs = stopwatch.ElapsedMilliseconds;
            history.ExecutedAt = DateTime.UtcNow;
            await historyRepository.AddAsync(history, context.CancellationToken);
        }
    }

    private static string? Truncate(string? value, int maxLength)
    {
       return string.IsNullOrEmpty(value) || value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}