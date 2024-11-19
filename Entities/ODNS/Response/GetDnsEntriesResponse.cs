using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ODNS.Response
{
    public class GetDnsEntriesResponse : StatusCode
    {
        public MetaData? metaData {  get; set; }
        public List<DnsEntry> DnsEntries { get; set; } = new List<DnsEntry>();
    }
}
