using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ODNS.Request;
using Entities.ODNS.Response;

namespace ODNSRepository.Repository
{
    public interface IOdnsRepository
    {
        Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request);
    }
}
