using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics.Entities
{
    public class GetIPGeoInfo_Response
    {
        public string? geo { get; set; } = "None";
        public string? org { get; set; } = "None";
        public int? asn { get; set; } = -1;
        public string? prefix {  get; set; } = "None";
    }

    public class GetIPGeoInfo_Request
    {
        public string? ip { get; set; }
    }
}
