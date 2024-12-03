using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.ODNS.Request
{
    public class DnsEntryFilter
    {
        public EProtocol? protocol {  get; set; }
        [JsonPropertyName("QueriedIP")]
        public string? ip_request { get; set; }
        [JsonPropertyName("ReplyingIP")]
        public string? ip_response { get; set; }
        [JsonPropertyName("BackendResolver")]
        public string?  a_record { get; set; }
        public string? timestamp_request { get; set; }
        public string? timestamp_response { get; set; }
        [JsonPropertyName("ResolverType")]
        public EResponseType? response_type { get; set; }
        [JsonPropertyName("QueriedIP_Country")]
        public string? country_request { get; set; }
        [JsonPropertyName("ReplyingIP_Country")]
        public string? country_response { get; set; }
        [JsonPropertyName("QueriedIP_ASN")]
        public int? asn_request { get; set; }
        [JsonPropertyName("ReplyingIP_ASN")]
        public int? asn_response { get; set; }
        public string? prefix_request { get; set; }
        public string? prefix_response { get; set; }
        [JsonPropertyName("QueriedIP_Org")]
        public string? org_request { get; set; }
        [JsonPropertyName("ReplyingIP_Org")]
        public string? org_response { get; set; }
        [JsonPropertyName("BackendResolver_Country")]
        public string? country_arecord { get; set; }
        [JsonPropertyName("BackendResolver_ASN")]
        public int? asn_arecord { get; set; }
        public string? prefix_arecord { get;set; }
        [JsonPropertyName("BackendResolver_Org")]
        public string? org_arecord { get; set; }
    }
}
