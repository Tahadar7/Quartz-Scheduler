using Mapster;
using QuartzScheduler.Data.Entities;
using QuartzScheduler.Shared.DTOs.History;
using QuartzScheduler.Shared.DTOs.Jobs;

namespace QuartzScheduler.API.Mapping;

// Implements IRegister so Mapster picks this up via an assembly scan
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Requests to Entities
        config.NewConfig<CreateEmailJobRequest, EmailJob>();
        config.NewConfig<CreateHttpJobRequest, HttpJob>();

        // Update maps onto an EXISTING tracked entity
        config.NewConfig<UpdateEmailJobRequest, EmailJob>()
              .Ignore(dest => dest.Id)
              .Ignore(dest => dest.CreatedAt)
              .Ignore(dest => dest.ExecutionHistory);

        config.NewConfig<UpdateHttpJobRequest, HttpJob>()
              .Ignore(dest => dest.Id)
              .Ignore(dest => dest.CreatedAt)
              .Ignore(dest => dest.ExecutionHistory);

        // Entities to Responses

        config.NewConfig<EmailJob, EmailJobResponse>()
              .Ignore(dest => dest.NextRunTime);   // comes from the scheduler, not the entity

        config.NewConfig<HttpJob, HttpJobResponse>()
              .Ignore(dest => dest.NextRunTime);

        // Target means different things for different sub types ToAddress for EmailJob, Url for HttpJob
        config.NewConfig<Job, JobSummaryResponse>()
              .Map(dest => dest.Target, src => ResolveTarget(src))
              .Ignore(dest => dest.CronDescription)     // computed from the cron string
              .Ignore(dest => dest.NextRunTime)         // from the Quartz scheduler
              .Ignore(dest => dest.LastRunSucceeded);   // from the history table

        config.NewConfig<JobExecutionHistory, JobExecutionResponse>()
              .Map(dest => dest.JobName, src => src.Job != null ? src.Job.JobName : string.Empty);
    }

    // TPH resolution for both types: the recipient for email jobs, the URL for http jobs
    private static string? ResolveTarget(Job job) => job switch
    {
        EmailJob email => email.ToAddress,
        HttpJob http => http.Url,
        _ => null
    };
}