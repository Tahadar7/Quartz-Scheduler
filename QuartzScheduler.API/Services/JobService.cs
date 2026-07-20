using FluentValidation;
using Mapster;
using QuartzScheduler.API.Exceptions;
using QuartzScheduler.Data.Entities;
using QuartzScheduler.Data.Repositories;
using QuartzScheduler.Shared.DTOs.Jobs;

namespace QuartzScheduler.API.Services;
public class JobService(
    IJobRepository jobRepository,
    IJobHistoryRepository historyRepository,
    IValidator<CreateEmailJobRequest> createEmailValidator,
    IValidator<CreateHttpJobRequest> createHttpValidator,
    IValidator<UpdateEmailJobRequest> updateEmailValidator,
    IValidator<UpdateHttpJobRequest> updateHttpValidator) : IJobService
{
    public async Task<IEnumerable<JobSummaryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var jobs = await jobRepository.GetAllAsync(ct);
        var summaries = jobs.Adapt<List<JobSummaryResponse>>();

        // fields Mapster can't populate from the entity alone
        foreach (var summary in summaries)
        {
            var lastRun = await historyRepository.GetLatestByJobIdAsync(summary.Id, ct);
            summary.LastRunSucceeded = lastRun?.IsSuccess;
        }

        return summaries;
    }

    public async Task<EmailJobResponse?> GetEmailJobByIdAsync(int id, CancellationToken ct = default)
    {
        var job = await jobRepository.GetEmailJobByIdAsync(id, ct);
        return job?.Adapt<EmailJobResponse>();
    }

    public async Task<HttpJobResponse?> GetHttpJobByIdAsync(int id, CancellationToken ct = default)
    {
        var job = await jobRepository.GetHttpJobByIdAsync(id, ct);
        return job?.Adapt<HttpJobResponse>();
    }

    public async Task<EmailJobResponse> CreateEmailJobAsync(CreateEmailJobRequest request, CancellationToken ct = default)
    {
        await createEmailValidator.ValidateAndThrowAsync(request, ct);

        if (await jobRepository.ExistsByNameAsync(request.JobName, ct)) {
            throw new ConflictException("A job with this name already exists.");
        }

        var job = request.Adapt<EmailJob>();
        await jobRepository.AddAsync(job, ct);

        return job.Adapt<EmailJobResponse>();
    }

    public async Task<HttpJobResponse> CreateHttpJobAsync(CreateHttpJobRequest request, CancellationToken ct = default)
    {
        await createHttpValidator.ValidateAndThrowAsync(request, ct);

        if (await jobRepository.ExistsByNameAsync(request.JobName, ct)) {
            throw new ConflictException("A job with this name already exists.");
        }

        var job = request.Adapt<HttpJob>();
        await jobRepository.AddAsync(job, ct);

        return job.Adapt<HttpJobResponse>();
    }

    public async Task<EmailJobResponse?> UpdateEmailJobAsync(int id, UpdateEmailJobRequest request, CancellationToken ct = default)
    {
        await updateEmailValidator.ValidateAndThrowAsync(request, ct);

        var job = await jobRepository.GetEmailJobByIdAsync(id, ct);
        if (job is null) {
            return null;
        }

        if (await jobRepository.ExistsByNameAsync(request.JobName, id, ct)) {
            throw new ConflictException("A job with this name already exists.");
        }

        request.Adapt(job);   // maps onto the tracked entity in place
        await jobRepository.UpdateAsync(job, ct);

        return job.Adapt<EmailJobResponse>();
    }

    public async Task<HttpJobResponse?> UpdateHttpJobAsync(int id, UpdateHttpJobRequest request, CancellationToken ct = default)
    {
        await updateHttpValidator.ValidateAndThrowAsync(request, ct);

        var job = await jobRepository.GetHttpJobByIdAsync(id, ct);
        if (job is null) {
            return null;
        }

        if (await jobRepository.ExistsByNameAsync(request.JobName, id, ct)) {
            throw new ConflictException("A job with this name already exists.");
        }

        request.Adapt(job);
        await jobRepository.UpdateAsync(job, ct);

        return job.Adapt<HttpJobResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var job = await jobRepository.GetByIdAsync(id, ct);
        if (job is null) {
            return false;
        }

        await jobRepository.DeleteAsync(job, ct);

        return true;
    }
}