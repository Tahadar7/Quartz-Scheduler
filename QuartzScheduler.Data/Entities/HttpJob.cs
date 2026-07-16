using QuartzScheduler.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzScheduler.Data.Entities
{
    public class HttpJob : Job
    {
        [Required]
        [MaxLength(2048)]
        public string Url { get; set; } = string.Empty;
        public HttpMethodType HttpMethod { get; set; } = HttpMethodType.GET;

        // JSON string of header key/value pairs
        public string? Headers { get; set; }
        public string? RequestBody { get; set; }

        // override the JobType property to return JobType.Http
        public override JobType JobType => JobType.Http;
    }

}
