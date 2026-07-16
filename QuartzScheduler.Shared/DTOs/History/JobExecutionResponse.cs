using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.History
{
    public class JobExecutionResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobName { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }          // for http jobs
        public string? ResponseSummary { get; set; }  

        public DateTime ExecutedAt { get; set; }
        public long DurationMs { get; set; }          // how long the job took
    }
}