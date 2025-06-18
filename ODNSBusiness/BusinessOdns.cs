using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.ODNS;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ODNSRepository;
using ODNSRepository.Repository;
using System.Reflection;

namespace ODNSBusiness
{
    public class BusinessOdns : IBusinessOdns
    {
        private readonly ILogger<BusinessOdns> _logger;
        private readonly IConfiguration _configuration;
        private IMetricsManager _metricsManager;
        private IOdnsRepository _dnsRepository;
        public BusinessOdns(ILogger<BusinessOdns> logger, IConfiguration config , IOdnsRepositoryFactory odnsRepositoryFactory, IMetricsManager metricsManager)
        {
            _configuration = config;
            _logger = logger;
            _metricsManager = metricsManager;
            string dbtype = _configuration["Database:dbtype"];
            _dnsRepository = odnsRepositoryFactory.GetInstance(dbtype);
        }

        public async Task<GetDnsEntriesResponse> GetDnsEntries(IGetDnsEntriesRequest request, string forwardedForIp)
        {
            _logger.LogInformation($"GetDnsEntries called rid: {request.rid} with the following request:\n {JsonSerializer.Serialize(request)}");
            GetDnsEntriesResponse response = new GetDnsEntriesResponse();
            try
            {
                _metricsManager.IncrementRequestCounter("GetDnsEntries", forwardedForIp);
                request.fixSortField();
                response = await _dnsRepository.GetDnsEntries(request);

                if(request is GetDnsEntriesRequestV2 requestV2)
                {
                    if (requestV2.fieldsToReturn.Count > 0)
                    {
                        foreach (var dnsEntry in response.dnsEntries)
                        {
                            KeepOnlySpecifiedFields(dnsEntry, requestV2.fieldsToReturn);
                        }
                    }
                    
                }
                _logger.LogDebug($"GetDnsEntries response for rid: {request.rid}\n {JsonSerializer.Serialize(response)}");
            }
            catch (Exception ex) 
            {
                response.statusCode.code = -1;
                response.statusCode.message = "Error please try again later";
                _logger.LogError($"Error occured in {nameof(this.GetDnsEntries)} with exception: {ex.ToString()}");
            }
            return response;
        }


        public static void KeepOnlySpecifiedFields<T>(T obj, List<string> fieldsToKeep)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                // Get the JsonPropertyName if set
                var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                var jsonName = jsonAttr?.Name ?? prop.Name;

                // If it's NOT in the keep list, set it to null
                if (!fieldsToKeep.Contains(jsonName, StringComparer.OrdinalIgnoreCase))
                {
                    prop.SetValue(obj, null);
                }
            }
        }

    }
}
