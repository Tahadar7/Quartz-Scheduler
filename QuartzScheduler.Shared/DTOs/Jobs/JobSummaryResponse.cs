using QuartzScheduler.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.Jobs
{
    public class JobSummaryResponse
    {
        public int Id { get; set; }
        public string JobName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public JobType JobType { get; set; }

        public string CronExpression { get; set; } = string.Empty;
        public string? CronDescription { get; set; }   // human-readable

        public bool IsActive { get; set; }

        public DateTime? LastExecutedAt { get; set; }
        public DateTime? NextRunTime { get; set; }
        public bool? LastRunSucceeded { get; set; }     
        public string? Target { get; set; }
    }
}
