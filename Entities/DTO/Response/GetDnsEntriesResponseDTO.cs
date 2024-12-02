using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ODNS.Response;

namespace Entities.DTO.Response
{
    public class GetDnsEntriesResponseDTO
    {
        public MetaData? metaData { get; set; }
        public List<DnsEntryDTO> dnsEntries { get; set; } = new List<DnsEntryDTO>();

        public GetDnsEntriesResponse GetDisplayResponse()
        {
            GetDnsEntriesResponse response = new GetDnsEntriesResponse();
            response.metaData = metaData;
            // To-Do later maybe a parallization would be best for big number of elements
            response.dnsEntries = this.dnsEntries
                .Select(x => x.GetDisplayResponse())
                .ToList();
            return response;
        }
    }
}
