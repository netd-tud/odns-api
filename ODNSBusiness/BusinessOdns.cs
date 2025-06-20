﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Entities.ODNS;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Metrics;
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

        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request, string forwardedForIp)
        {
            _logger.LogInformation($"GetDnsEntries called rid: {request.rid} with the following request:\n {JsonSerializer.Serialize(request)}");
            GetDnsEntriesResponse response = new GetDnsEntriesResponse();
            try
            {
                _metricsManager.IncrementRequestCounter("GetDnsEntries", forwardedForIp);
                request.fixSortField();
                response = await _dnsRepository.GetDnsEntries(request);
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
    }
}
