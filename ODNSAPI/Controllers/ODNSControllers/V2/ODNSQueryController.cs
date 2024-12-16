using Asp.Versioning;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ODNSBusiness;

namespace ODNSAPI.Controllers.ODNSControllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    //[Route("[controller]/[action]")]
    [ApiVersion(1.1)]
    public class ODNSQueryController : ControllerBase
    {
        private readonly ILogger<ODNSQueryController> _logger;
        private IBusinessOdns _businessOdns;
        public ODNSQueryController(ILogger<ODNSQueryController> logger, IBusinessOdns businessOdns)
        {
            _logger = logger;
            _businessOdns = businessOdns;
        }
        /// <summary>
        /// Endpoint used to retrieve the DNS entries from the ODNS project
        /// </summary>
        /// <remarks>
        /// All the request body parameters are optional.
        /// 
        /// You can filter by providing the filter object and the search condition per property.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///        "pagination": {
        ///          "page": 1,
        ///          "per_page": 10
        ///        },
        ///        "filter": {
        ///          "protocol": "tcp",
        ///          "backend_resolver_country": "USA"
        ///        },
        ///          "sort": {
        ///            "field": "timestamp_request",
        ///            "order": "desc"
        ///          }
        ///     }
        /// 
        /// Note: timestamps are of the following format yyyy-MM-dd hh:mm:ss.ms
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [HttpPost]
        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request)
        {
            GetDnsEntriesResponse response = await _businessOdns.GetDnsEntries(request);
            return response;
            //return await _businessOdns.GetDnsEntries(request);
        }

    }
}
