using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuartzScheduler.Data.Context;
using QuartzScheduler.Data.Entities;

namespace QuartzScheduler.Data.Repositories;
public class JobRepository(ApplicationDbContext context) : IJobRepository
{
    public async Task<IEnumerable<Job>> GetAllAsync(CancellationToken ct = default)
    {
      return await context.Jobs
            .AsNoTracking()
            .OrderBy(j => j.JobName)
            .ToListAsync(ct);
    }

    public async Task<Job?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<EmailJob?> GetEmailJobByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Jobs
            .OfType<EmailJob>()
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<HttpJob?> GetHttpJobByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Jobs
            .OfType<HttpJob>()
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<IEnumerable<Job>> GetActiveJobsAsync(CancellationToken ct = default)
    {
        return await context.Jobs
            .AsNoTracking()
            .Where(j => j.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Job job, CancellationToken ct = default)
    {
        context.Jobs.Add(job);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Job job, CancellationToken ct = default)
    {
        context.Jobs.Update(job);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Job job, CancellationToken ct = default)
    {
        context.Jobs.Remove(job);
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string jobName, CancellationToken ct = default)
    {
        return await context.Jobs.AnyAsync(j => j.JobName == jobName, ct);
    }

    public async Task<bool> ExistsByNameAsync(string jobName, int excludeId, CancellationToken ct = default)
    {
        return await context.Jobs.AnyAsync(j => j.JobName == jobName && j.Id != excludeId, ct);
    }
}

