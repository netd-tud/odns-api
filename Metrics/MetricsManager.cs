using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Metrics
{
    public class MetricsManager:IMetricsManager
    {
        private readonly IConfiguration _configuration;
        private Meter _customMetrics;

        private Counter<long> _requestCounterMetric;

        public MetricsManager(IConfiguration config) 
        {
            _configuration = config;
            _customMetrics = new Meter(_configuration.GetSection("Metrics:MeterName").Value, _configuration.GetSection("Metrics:MeterVersion").Value);
            initializeMetrics();
        }

        private void initializeMetrics()
        {
            _requestCounterMetric = _customMetrics.CreateCounter<long>(_configuration.GetSection("Metrics:CustomMetrics:RequestCounter").Value);
        }

        public void incrementRequestCounter(string route) 
        {
            TagList tagsList = new TagList();
            tagsList.Add("route", route);
            //tagsList.Add("lat", 52.5200);
            //tagsList.Add("lon", 13.4050);
            tagsList.Add("lookup", "DE");
            _requestCounterMetric.Add(1, tagsList);
        }

    }
}
