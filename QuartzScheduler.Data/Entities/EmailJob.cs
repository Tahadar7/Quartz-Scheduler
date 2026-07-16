using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using QuartzScheduler.Shared.Enums;

namespace QuartzScheduler.Data.Entities
{
    public class EmailJob : Job
    {
        [Required]
        [MaxLength(256)]
        public string ToAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;

        // Override the JobType property to return JobType.Email
        public override JobType JobType => JobType.Email;
    }
}
