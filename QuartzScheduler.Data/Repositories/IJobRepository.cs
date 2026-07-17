using System;
using System.Collections.Generic;
using System.Text;
using QuartzScheduler.Data.Entities;

namespace QuartzScheduler.Data.Repositories;
public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync(CancellationToken ct = default);
    Task<Job?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmailJob?> GetEmailJobByIdAsync(int id, CancellationToken ct = default);
    Task<HttpJob?> GetHttpJobByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Job>> GetActiveJobsAsync(CancellationToken ct = default);
    Task AddAsync(Job job, CancellationToken ct = default);      // accepts any subtype
    Task UpdateAsync(Job job, CancellationToken ct = default);
    Task DeleteAsync(Job job, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string jobName, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string jobName, int excludeId, CancellationToken ct = default);
}
