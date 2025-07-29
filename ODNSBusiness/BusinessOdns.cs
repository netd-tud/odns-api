using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CustomExceptions;
using Entities.ODNS;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Entities.Auth;
using Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ODNSRepository;
using ODNSRepository.Repository;
using Utilities;

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
            GetDnsEntriesRequestV2? r2 = null;
            try
            {
                request.fixSortField();
                if (request is GetDnsEntriesRequestV2 requestV2)
                {
                    r2 = requestV2;
                    CheckFieldsToKeep(typeof(DnsEntry), requestV2.fieldsToReturn);
                }
                _metricsManager.IncrementRequestCounter("GetDnsEntries", forwardedForIp);

                response = await _dnsRepository.GetDnsEntries(request);

                if(r2 != null)
                {
                    
                    if (r2.fieldsToReturn.Count > 0)
                    {
                        foreach (var dnsEntry in response.dnsEntries)
                        {
                            KeepOnlySpecifiedFields(dnsEntry, r2.fieldsToReturn);
                        }
                    }
                    
                }
                _logger.LogDebug($"GetDnsEntries response for rid: {request.rid}\n {JsonSerializer.Serialize(response)}");
            }
            catch(AmbiguousSortFieldException ex)
            {
                response.statusCode.code = -1;
                response.statusCode.message = ex.Message;
                _logger.LogError($"Error occured in {nameof(this.GetDnsEntries)} with exception: {ex.ToString()}");
            }
            catch(AggregateException aex)
            {
                response.statusCode.code = -1;
                response.statusCode.message = "";
                foreach (var  ex in aex.InnerExceptions)
                {
                    if( ex is AmbiguousSortFieldException fex)
                    {
                        response.statusCode.message += $"{fex.Message} " ;
                    }
                }
                _logger.LogError($"Error occured in {nameof(this.GetDnsEntries)} with exception: {aex.ToString()}");

            }
            catch (Exception ex) 
            {
                response.statusCode.code = -1;
                response.statusCode.message = "Error please try again later";
                _logger.LogError($"Error occured in {nameof(this.GetDnsEntries)} with exception: {ex.ToString()}");
            }
            return response;
        }

        private class PropertyMetadata
        {
            public string PropertyName { get; set; }
            public string JsonName { get; set; }
        }

        public void CheckFieldsToKeep(Type type, List<string>? fieldsToKeep)
        {
            if (fieldsToKeep == null && !fieldsToKeep.Any())
                return;
            
            List<PropertyMetadata> properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new PropertyMetadata
            {
                PropertyName = p.Name,
                JsonName = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
            })
            .ToList();

            List<AmbiguousSortFieldException> exceptions = new List<AmbiguousSortFieldException>();

            foreach(string fieldToKeep in fieldsToKeep)
            {
                try
                {
                    ValidateSingleField(fieldToKeep, properties);
                }
                catch (AmbiguousSortFieldException ex) 
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
        private void ValidateSingleField(string field, List<PropertyMetadata> properties)
        {
            var originalField = field;

            // 1. Exact Match
            foreach (var prop in properties)
            {
                if (string.Equals(prop.JsonName ?? prop.PropertyName, originalField, StringComparison.OrdinalIgnoreCase))
                {
                    field = prop.JsonName ?? prop.PropertyName; // Correct to canonical name
                    return; // Success
                }
            }

            // 2. Fuzzy Match
            string bestMatch = string.Empty;
            int minDistance = int.MaxValue;
            const int similarityThreshold = 2;
            FuzzyMatching fm = new FuzzyMatching(FuzzyMatchingAlgo.LEVENSHTEIN);
            foreach (var prop in properties)
            {
                int distanceProp = fm.FuzzyMatch(originalField, prop.JsonName ?? prop.PropertyName);
                if (distanceProp < minDistance)
                {
                    minDistance = distanceProp;
                    bestMatch = prop.JsonName ?? prop.PropertyName;
                }

                //if (prop.JsonName != null)
                //{
                //    int distanceJson = fm.FuzzyMatch(originalField, prop.JsonName);
                //    if (distanceJson < minDistance)
                //    {
                //        minDistance = distanceJson;
                //        bestMatch = prop.PropertyName;
                //    }
                //}
            }

            // 3. Evaluate and throw
            if (!string.IsNullOrEmpty(bestMatch) && minDistance <= similarityThreshold)
            {
                throw new AmbiguousSortFieldException(originalField, bestMatch);
            }
            else
            {
                throw new AmbiguousSortFieldException(originalField);
            }
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

        public async Task<StatusCode> RequestApiKey(ApiKeyRecordIn apiInfo)
        {
            StatusCode response = new StatusCode();
            try
            {
                ApiKeyRecord result = await _dnsRepository.InsertApiKeyRecord(apiInfo);
                if (result != null && result.api_key != null) 
                {
                    response.statusCode.message = "You will receive the api key by email shortly";
                }
            }
            catch (Exception ex) 
            {
                response.statusCode.code = 1;
                response.statusCode.message = "Error please try again later";
            }
            return response;
        }
    }
}
