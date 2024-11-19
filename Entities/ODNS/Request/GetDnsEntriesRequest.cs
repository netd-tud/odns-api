using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ODNS.Request
{
    public class GetDnsEntriesRequest
    {
        public string rid { get; set; } = Guid.NewGuid().ToString();
        public Pagination pagination {  get; set; } = new Pagination();
        public DnsEntryFilter? filter { get; set; }
        public Sort? sort { get; set; }
    }
}
