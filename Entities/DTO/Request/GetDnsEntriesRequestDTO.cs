using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ODNS.Request;

namespace Entities.DTO.Request
{
    public class GetDnsEntriesRequestDTO
    {
        public string rid { get; set; } 
        public Pagination pagination { get; set; } = new Pagination();
        public DnsEntryFilterDTO? filter { get; set; }
        public Sort? sort { get; set; }
    }
}
