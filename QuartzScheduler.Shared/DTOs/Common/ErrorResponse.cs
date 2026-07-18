using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Shared.DTOs.Common;
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    // field-level errors from validation
    public IDictionary<string, string[]>? Errors { get; set; }
}
