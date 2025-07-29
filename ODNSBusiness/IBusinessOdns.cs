using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Auth;
using Entities.ODNS.Request;
using Entities.ODNS.Response;

namespace ODNSBusiness
{
    public interface IBusinessOdns
    {
        Task<GetDnsEntriesResponse> GetDnsEntries(IGetDnsEntriesRequest request,string forwardedForIp);
        Task<StatusCode> RequestApiKey(ApiKeyRecordIn apiInfo);
    }
}
