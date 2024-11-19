using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ODNS.Response
{
    public class StatusCodeInternal
    {
        public string? message { get; set; } = "Success";
        public int code { get; set; } = 0;
    }
    public class StatusCode
    {
        public StatusCodeInternal? statusCode { get; set; } = new StatusCodeInternal();
    }
}
