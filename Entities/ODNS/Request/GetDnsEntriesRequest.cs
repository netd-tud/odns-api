using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Utilities;
using CustomExceptions;

namespace Entities.ODNS.Request
{
    public interface IGetDnsEntriesRequest
    {
        [SwaggerIgnore]
        public string rid { get; set; }
        [SwaggerIgnore]
        public bool latest { get; set; } 
        public Pagination pagination { get; set; } //= new Pagination();
        public DnsEntryFilter? filter { get; set; }
        public Sort? sort { get; set; }

        public void fixSortField();
    }

    public class GetDnsEntriesRequest: IGetDnsEntriesRequest
    {
        [SwaggerIgnore]
        public string rid { get; set; } = Guid.NewGuid().ToString();
        [SwaggerIgnore]
        public bool latest { get; set; } = false;
        public Pagination pagination {  get; set; } //= new Pagination();
        public DnsEntryFilter? filter { get; set; }
        public Sort? sort { get; set; }

        public void fixSortField()
        {
            if(this.sort == null)
                return;

            PropertyInfo[] fields = typeof(DnsEntryFilter).GetProperties(BindingFlags.Public|BindingFlags.Instance );
            foreach (var field in fields)
            {
                // Check if the field has the CustomAttribute
                var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attribute != null)
                {
                    if (this.sort.field.ToLower() == attribute.Name.ToLower())
                    {
                        this.sort.field = field.Name;
                        break;
                    }
                    //Console.WriteLine($"Field: {field.Name}, Attribute Description: {attribute.Name}");
                }
            }
        }
    }

    public class GetDnsEntriesRequestV2: IGetDnsEntriesRequest
    {
        [SwaggerIgnore]
        public string rid { get; set; } = Guid.NewGuid().ToString();
        [SwaggerIgnore]
        public bool latest { get; set; } = false;
        public Pagination pagination { get; set; } //= new Pagination();
        public DnsEntryFilter? filter { get; set; }
        public Sort? sort { get; set; }
        public List<string>? fieldsToReturn { get; set; } = new List<string>();

        public void fixSortField()
        {
            if (string.IsNullOrWhiteSpace(sort?.field))
                return;

            var filterProperties = typeof(DnsEntryFilter)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new
            {
                PropertyName = p.Name,
                JsonName = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
            })
            .ToList();

            // === 1. First Pass: Exact (Case-Insensitive) Match ===
            foreach (var prop in filterProperties)
            {
                // Check against C# property name (e.g., "DnsName")
                if (string.Equals(prop.PropertyName, sort.field, StringComparison.OrdinalIgnoreCase))
                {
                    sort.field = prop.PropertyName;
                    return; // Exact match found, we are done.
                }
                // Check against JSON name (e.g., "dns_name")
                if (prop.JsonName != null && string.Equals(prop.JsonName, sort.field, StringComparison.OrdinalIgnoreCase))
                {
                    sort.field = prop.PropertyName;
                    return; // Exact match found, we are done.
                }
            }



            // === 2. Second Pass: Fuzzy Matching (if no exact match was found) ===
            string bestMatch = string.Empty;
            int minDistance = int.MaxValue;
            FuzzyMatching fm = new FuzzyMatching(FuzzyMatchingAlgo.LEVENSHTEIN);
            // You can adjust this threshold. A lower value means the match must be more similar.
            // A threshold of 2 allows for up to two typos.
            const int similarityThreshold = 2;

            foreach (var prop in filterProperties)
            {
                // Compare against C# property name
                int distanceProp = fm.FuzzyMatch(sort.field, prop.JsonName ?? prop.PropertyName);
                if (distanceProp < minDistance)
                {
                    minDistance = distanceProp;
                    bestMatch = prop.JsonName ?? prop.PropertyName;
                }

                // Compare against JSON name, if it exists
                //if (prop.JsonName != null)
                //{
                //    int distanceJson = fm.FuzzyMatch(sort.field, prop.JsonName);
                //    if (distanceJson < minDistance)
                //    {
                //        minDistance = distanceJson;
                //        bestMatch = prop.JsonName ?? prop.PropertyName;
                //    }
                //}
            }

            // Only accept the fuzzy match if it's "close enough"
            if (minDistance <= similarityThreshold)
            {
                throw new AmbiguousSortFieldException(sort.field, bestMatch);
            }
            // If the closest match is still too different, you might choose to leave the field as is,
            // clear it, or throw an exception, depending on your desired behavior.

            throw new AmbiguousSortFieldException(sort.field);
            
        }
    }
}
