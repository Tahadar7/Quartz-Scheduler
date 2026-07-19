using QuartzScheduler.Shared.DTOs.Jobs;
using FluentValidation;
namespace QuartzScheduler.API.Services;
public interface IJobService
{
    Task<IEnumerable<JobSummaryResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EmailJobResponse?> GetEmailJobByIdAsync(int id, CancellationToken ct = default);
    Task<HttpJobResponse?> GetHttpJobByIdAsync(int id, CancellationToken ct = default);
    Task<EmailJobResponse> CreateEmailJobAsync(IValidator<CreateEmailJobRequest> createEmailValidator,CreateEmailJobRequest request, CancellationToken ct = default);
    Task<HttpJobResponse> CreateHttpJobAsync(IValidator<CreateHttpJobRequest> createHttpValidator,CreateHttpJobRequest request, CancellationToken ct = default);
    Task<EmailJobResponse?> UpdateEmailJobAsync(IValidator<UpdateEmailJobRequest> updateEmailValidator, int id, UpdateEmailJobRequest request, CancellationToken ct = default);
    Task<HttpJobResponse?> UpdateHttpJobAsync(IValidator<UpdateHttpJobRequest> updateHttpValidator, int id, UpdateHttpJobRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}