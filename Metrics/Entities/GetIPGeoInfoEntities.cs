using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics.Entities
{
    public class GetIPGeoInfo_Response
    {
        public string? geo {  get; set; }
        public string? org { get; set; }
        public int? asn { get; set; }
        public string? prefix {  get; set; }
    }

    public class GetIPGeoInfo_Request
    {
        public string ip { get; set; }
    }
}
