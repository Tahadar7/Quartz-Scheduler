using QuartzScheduler.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzScheduler.Data.Entities
{
    // TPH (table per hierarchy) inheritance is used to store all jobs in a single table
    // JobType enum is used as the discriminator column
    public abstract class Job : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string JobName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string CronExpression { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastExecutedAt { get; set; }

        // Each job type will have its own JobType enum value, each subclass overrides it
        public abstract JobType JobType { get; }

        // one job has many execution records
        public ICollection<JobExecutionHistory> ExecutionHistory { get; set; } = new List<JobExecutionHistory>();

    }
}
