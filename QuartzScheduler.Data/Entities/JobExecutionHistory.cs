using System.ComponentModel.DataAnnotations;

namespace QuartzScheduler.Data.Entities
{
    public class JobExecutionHistory : BaseEntity
    {
        // Foreign key to the Job entity
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public bool IsSuccess { get; set; }

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        // http-specific outcome fields (null for email jobs)
        public int? StatusCode { get; set; }

        [MaxLength(2000)]
        public string? ResponseSummary { get; set; }   

        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

        public long DurationMs { get; set; }            // how long the run took
    }
}