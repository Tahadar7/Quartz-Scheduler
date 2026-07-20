using QuartzScheduler.Shared.DTOs.Jobs;
using FluentValidation;
namespace QuartzScheduler.API.Services;
public interface IJobService
{
    Task<IEnumerable<JobSummaryResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EmailJobResponse?> GetEmailJobByIdAsync(int id, CancellationToken ct = default);
    Task<HttpJobResponse?> GetHttpJobByIdAsync(int id, CancellationToken ct = default);
    Task<EmailJobResponse> CreateEmailJobAsync(CreateEmailJobRequest request, CancellationToken ct = default);
    Task<HttpJobResponse> CreateHttpJobAsync(CreateHttpJobRequest request, CancellationToken ct = default);
    Task<EmailJobResponse?> UpdateEmailJobAsync(int id, UpdateEmailJobRequest request, CancellationToken ct = default);
    Task<HttpJobResponse?> UpdateHttpJobAsync(int id, UpdateHttpJobRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}