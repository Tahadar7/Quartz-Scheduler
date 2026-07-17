using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuartzScheduler.Data.Entities;


namespace QuartzScheduler.Data.Context { 

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobExecutionHistory> JobExecutionHistory => Set<JobExecutionHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>(entity =>
        {
            // TPH: one table, a discriminator column 
            entity.HasDiscriminator<string>("JobKind")
                  .HasValue<EmailJob>("Email")
                  .HasValue<HttpJob>("Http");

            entity.Property(j => j.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            // unique job name
            entity.HasIndex(j => j.JobName).IsUnique();
        });

        modelBuilder.Entity<HttpJob>(entity =>
        {
            // store the HTTP method enum as string
            entity.Property(h => h.HttpMethod)
                  .HasConversion<string>()
                  .HasMaxLength(10);
        });

        modelBuilder.Entity<JobExecutionHistory>(entity =>
        {
            entity.Property(h => h.ExecutedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            // one job many history records
            entity.HasOne(h => h.Job)
                  .WithMany(j => j.ExecutionHistory)
                  .HasForeignKey(h => h.JobId)
                  .OnDelete(DeleteBehavior.Cascade); // deleting a job removes its history

            entity.HasIndex(h => h.ExecutedAt);
        });
    }
}
}

