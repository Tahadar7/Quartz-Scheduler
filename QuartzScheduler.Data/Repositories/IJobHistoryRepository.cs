using System;
using System.Collections.Generic;
using System.Text;
using QuartzScheduler.Data.Entities;

namespace QuartzScheduler.Data.Repositories;

public interface IJobHistoryRepository
{
    Task AddAsync(JobExecutionHistory history, CancellationToken ct = default);

    // history for one job, newest first
    Task<IEnumerable<JobExecutionHistory>> GetByJobIdAsync(int jobId, CancellationToken ct = default);

    // recent runs across all jobs, newest first
    Task<IEnumerable<JobExecutionHistory>> GetRecentAsync(int take = 50, CancellationToken ct = default);
}
