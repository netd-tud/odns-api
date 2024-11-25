using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Entities.ODNS;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ODNSRepository;
using ODNSRepository.Repository;

namespace ODNSBusiness
{
    public class BusinessOdns : IBusinessOdns
    {
        private readonly ILogger<BusinessOdns> _logger;
        private readonly IConfiguration _configuration;
        
        private IOdnsRepository _dnsRepository;
        public BusinessOdns(ILogger<BusinessOdns> logger, IConfiguration config , IOdnsRepositoryFactory odnsRepositoryFactory)
        {
            _configuration = config;
            _logger = logger;

            string dbtype = _configuration["Database:dbtype"];
            _dnsRepository = odnsRepositoryFactory.GetInstance(dbtype);
        }

        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request)
        {
            _logger.LogDebug($"GetDnsEntries called rid: {request.rid} with the following request:\n {JsonSerializer.Serialize(request)}");
            GetDnsEntriesResponse response = new GetDnsEntriesResponse();
            try
            {
                response = await _dnsRepository.GetDnsEntries(request);
                _logger.LogDebug($"GetDnsEntries response for rid: {request.rid}\n {JsonSerializer.Serialize(response)}");
            }
            catch (Exception ex) 
            {
                response.statusCode.code = -1;
                response.statusCode.message = "Error please try again later";
                _logger.LogError($"Error occured while executing {nameof(this.GetDnsEntries)}\n ex: {ex.ToString()}");
            }
            return response;
        }
    }
}
