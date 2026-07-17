using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuartzScheduler.Data.Context;
using QuartzScheduler.Data.Entities;

namespace QuartzScheduler.Data.Repositories;
public class JobHistoryRepository(ApplicationDbContext context) : IJobHistoryRepository
{
    public async Task AddAsync(JobExecutionHistory history, CancellationToken ct = default)
    {
        context.JobExecutionHistory.Add(history);
        await context.SaveChangesAsync(ct);
    }
    public async Task<IEnumerable<JobExecutionHistory>> GetByJobIdAsync(int jobId, CancellationToken ct = default)
    {
        return await context.JobExecutionHistory
            .AsNoTracking()
            .Where(h => h.JobId == jobId)
            .OrderByDescending(h => h.ExecutedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JobExecutionHistory>> GetRecentAsync(int take = 50, CancellationToken ct = default) =>
        await context.JobExecutionHistory
            .AsNoTracking()
            .OrderByDescending(h => h.ExecutedAt)
            .Take(take)
            .ToListAsync(ct);
}
