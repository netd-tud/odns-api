using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Json;
using Metrics.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Metrics
{
    public class MetricsManager:IMetricsManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private Meter _customMetrics;

        private Counter<long> _requestCounterMetric;

        public MetricsManager(IConfiguration config,ILogger logger) 
        {
            _configuration = config;
            _logger = logger;
            _customMetrics = new Meter(_configuration.GetSection("Metrics:MeterName").Value, _configuration.GetSection("Metrics:MeterVersion").Value);
            initializeMetrics();
        }

        private void initializeMetrics()
        {
            _requestCounterMetric = _customMetrics.CreateCounter<long>(_configuration.GetSection("Metrics:CustomMetrics:RequestCounter").Value);
        }

        
        public async Task IncrementRequestCounter(string route,string ip)
        {
            TagList tagsList = new TagList();

            GetIPGeoInfo_Response response = await GetIPGeoInfo(ip);

            tagsList.Add("route", route);
            tagsList.Add("lookup", response.geo);
            tagsList.Add("org", response.org);
            _requestCounterMetric.Add(1, tagsList);
        }

        private async Task<GetIPGeoInfo_Response> GetIPGeoInfo(string ip)
        {
            GetIPGeoInfo_Response result = new GetIPGeoInfo_Response();
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                HttpResponseMessage response = await client.PostAsJsonAsync(_configuration.GetSection("Metrics:GeoIPExtractor").Value, new GetIPGeoInfo_Request() { ip = ip });
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<GetIPGeoInfo_Response>();
                }
            }
            catch (Exception ex) 
            {
                //?
                _logger.LogError($"Error occured in {nameof(this.GetIPGeoInfo)} with exception: \n{ex.ToString()}");
            }
            
            return result;
        }
    }
}
