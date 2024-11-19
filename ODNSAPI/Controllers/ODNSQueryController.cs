using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ODNSBusiness;

namespace ODNSAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ODNSQueryController : ControllerBase
    {
        private readonly ILogger<ODNSQueryController> _logger;
        private IBusinessOdns _businessOdns;
        public ODNSQueryController(ILogger<ODNSQueryController> logger, IBusinessOdns businessOdns)
        {
            _logger = logger;
            _businessOdns = businessOdns;
        }

        [EnableRateLimiting("fixed")]
        [HttpPost(Name = "GetDnsEntries")]
        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request)
        {
            GetDnsEntriesResponse response = await _businessOdns.GetDnsEntries(request);
            return response;
            //return await _businessOdns.GetDnsEntries(request);
        }

    }
}
