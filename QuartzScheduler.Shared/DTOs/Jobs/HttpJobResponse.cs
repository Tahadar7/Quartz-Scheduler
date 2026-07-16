using System;
using System.Collections.Generic;
using System.Text;
using QuartzScheduler.Shared.Enums;

namespace QuartzScheduler.Shared.DTOs.Jobs
{
    public class HttpJobResponse
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

        // http-specific
        public string Url { get; set; } = string.Empty;
        public HttpMethodType HttpMethod { get; set; }
        public string? Headers { get; set; }        // JSON string of header key/value pairs
        public string? RequestBody { get; set; }
    }
}
