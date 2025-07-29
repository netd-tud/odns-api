using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Auth;
using Entities.ODNS.Request;
using Entities.ODNS.Response;

namespace ODNSRepository.Repository
{
    public interface IOdnsRepository
    {
        Task<GetDnsEntriesResponse> GetDnsEntries(IGetDnsEntriesRequest request);
        Task<ApiKeyRecord> GetApiKeyRecord(ApiKeyDTO apiKey);
        Task<ApiKeyRecord> InsertApiKeyRecord(ApiKeyRecordIn data);
    }
}
