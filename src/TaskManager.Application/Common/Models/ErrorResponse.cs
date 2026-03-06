using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Common.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public string? TraceId { get; set; }
        public string? StackTrace { get; set; }
    }
}
