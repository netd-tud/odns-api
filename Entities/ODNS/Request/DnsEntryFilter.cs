using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.ODNS.Request
{
    /// <summary>
    /// DnsEntryFilter used to filter the dns records
    /// </summary>
    public class DnsEntryFilter
    {
        /// <summary>
        /// Protocol could be TCP or UDP
        /// </summary>
        public EProtocol? protocol {  get; set; }
        /// <summary>
        /// queried_ip represents the requested ip
        /// </summary>
        [JsonPropertyName("queried_ip")]
        public string? ip_request { get; set; }
        [JsonPropertyName("replying_ip")]
        public string? ip_response { get; set; }
        [JsonPropertyName("backend_resolver")]
        public string?  a_record { get; set; }
        public string? timestamp_request { get; set; }
        public string? timestamp_response { get; set; }
        [JsonPropertyName("resolver_type")]
        public EResponseType? response_type { get; set; }
        [JsonPropertyName("queried_ip_country")]
        public string? country_request { get; set; }
        [JsonPropertyName("replying_ip_country")]
        public string? country_response { get; set; }
        [JsonPropertyName("queried_ip_asn")]
        public int? asn_request { get; set; }
        [JsonPropertyName("replying_ip_asn")]
        public int? asn_response { get; set; }
        public string? prefix_request { get; set; }
        public string? prefix_response { get; set; }
        [JsonPropertyName("queried_ip_org")]
        public string? org_request { get; set; }
        [JsonPropertyName("replying_ip_org")]
        public string? org_response { get; set; }
        [JsonPropertyName("backend_resolver_country")]
        public string? country_arecord { get; set; }
        [JsonPropertyName("backend_resolver_asn")]
        public int? asn_arecord { get; set; }
        public string? prefix_arecord { get;set; }
        [JsonPropertyName("backend_resolver_org")]
        public string? org_arecord { get; set; }
    }
}
