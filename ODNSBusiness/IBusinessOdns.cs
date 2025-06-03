using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ODNS.Request;
using Entities.ODNS.Response;

namespace ODNSBusiness
{
    public interface IBusinessOdns
    {
        Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request,string forwardedForIp);
    }
}
