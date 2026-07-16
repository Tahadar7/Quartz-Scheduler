using QuartzScheduler.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.Jobs
{
    public class CreateHttpJobRequest
    {
        public string JobName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CronExpression { get; set; } = string.Empty;

        // http-specific
        public string Url { get; set; } = string.Empty;
        public HttpMethodType HttpMethod { get; set; } = HttpMethodType.GET;
        public string? Headers { get; set; }       
        public string? RequestBody { get; set; }
    }
}
