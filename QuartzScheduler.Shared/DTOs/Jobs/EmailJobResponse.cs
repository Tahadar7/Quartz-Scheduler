using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.Jobs
{
    public class EmailJobResponse
    {
        public int Id { get; set; }
        public string JobName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CronExpression { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastExecutedAt { get; set; }
        public DateTime? NextRunTime { get; set; }

        // email-specific
        public string ToAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
    }
}
