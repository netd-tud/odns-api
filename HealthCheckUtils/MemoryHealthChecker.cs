using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace HealthCheckUtils
{
    public class MemoryHealthChecker : IHealthCheck
    {
        private IConfiguration _config;
        public string Name = "memory_check";
        private long _threshold;
        public MemoryHealthChecker(IConfiguration config) 
        {
            _config = config;
            _threshold = 1024L * 1024L * 1024L * Int64.Parse(_config.GetSection("HealthCheck:MemoryThresholdInGB").Value);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            long allocated = GC.GetTotalMemory(forceFullCollection: false);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "AllocatedBytes", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
            };
            HealthStatus status = (allocated < _threshold) ? HealthStatus.Healthy : HealthStatus.Unhealthy;

            return Task.FromResult(
                new HealthCheckResult(
                    status,
                    description: "Reports degraded status if allocated bytes " +
                        $">= {_threshold} bytes.",
                    exception: null,
                    data: data
                )
            );
        }
    }
}
