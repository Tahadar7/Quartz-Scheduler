using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.Jobs
{
    public class UpdateEmailJobRequest
    {
        public string JobName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CronExpression { get; set; } = string.Empty;

        public string ToAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }
}
