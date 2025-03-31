using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ODNS.Response
{
    public class DnsEntry
    {
        /*
        public string? protocol { get; set; }
        public string? ip_request { get; set; }
        public string? ip_response { get; set; }
        public string? a_record { get; set; }
        public string? timestamp_request { get; set; }
        public string? timestamp_response { get; set; }
        public string? response_type { get; set; }
        public string? country_request { get; set; }
        public string? country_response { get; set; }
        public int? asn_request { get; set; }
        public int? asn_response { get; set; }
        public string? prefix_request { get; set; }
        public string? prefix_response { get; set; }
        public string? org_request { get; set; }
        public string? org_response { get; set; }
        public string? country_arecord { get; set; }
        public int? asn_arecord { get; set; }
        public string? prefix_arecord { get; set; }
        public string? org_arecord { get; set; }
        
        */

        /// <summary>
        /// Protocol used during the scan. Available protocolos are UDP and TCP.
        /// </summary>
        public string? protocol { get; set; }
        /// <summary>
        /// Queried IP address that somehow triggers a valid DNS response.
        /// </summary>
        [JsonPropertyName("queried_ip")]
        public string? ip_request { get; set; }
        /// <summary>
        /// IP address that replies when our scanner queries queried_ip.
        /// </summary>
        [JsonPropertyName("replying_ip")]
        public string? ip_response { get; set; }
        /// <summary>
        /// IP address of the resolver that queried our auth. nameserver.
        /// </summary>
        [JsonPropertyName("backend_resolver")]
        public string? a_record { get; set; }
        /// <summary>
        /// Timestamp of the scanners DNS request.
        /// </summary>
        public string? timestamp_request { get; set; }
        /// <summary>
        /// Timestamp of the arriving DNS response.
        /// </summary>
        public string? timestamp_response { get; set; }
        /// <summary>
        /// Open DNS classification (Resolver:= queried_ip == replying_ip == backend_resolver, Forwarder:= queried_ip == replying_ip != backend_resolver, Transparent Forwarder:= queried_ip != replying_ip)
        /// </summary>
        [JsonPropertyName("resolver_type")]
        public string? response_type { get; set; }
        /// <summary>
        /// ISO-3 country code of the queried IP address.
        /// </summary>
        [JsonPropertyName("queried_ip_country")]
        public string? country_request { get; set; }
        /// <summary>
        /// ISO-3 country code of the replying IP address.
        /// </summary>
        [JsonPropertyName("replying_ip_country")]
        public string? country_response { get; set; }
        /// <summary>
        /// AS-Number of the queried IP address.
        /// </summary>
        [JsonPropertyName("queried_ip_asn")]
        public int? asn_request { get; set; }
        /// <summary>
        /// AS-Number of the replying IP address.
        /// </summary>
        [JsonPropertyName("replying_ip_asn")]
        public int? asn_response { get; set; }
        /// <summary>
        /// Registered covering prefix for the queried IP address.
        /// </summary>
        [JsonPropertyName("queried_ip_prefix")]
        public string? prefix_request { get; set; }
        /// <summary>
        /// Registered covering prefix for the replying IP address.
        /// </summary>
        [JsonPropertyName("replying_ip_prefix")]
        public string? prefix_response { get; set; }
        /// <summary>
        /// Organization name of the queried IP address ASN.
        /// </summary>
        [JsonPropertyName("queried_ip_org")]
        public string? org_request { get; set; }
        /// <summary>
        /// Organization name of the replying IP address ASN.
        /// </summary>
        [JsonPropertyName("replying_ip_org")]
        public string? org_response { get; set; }
        /// <summary>
        /// ISO-3 country code of the backend resolver IP address.
        /// </summary>
        [JsonPropertyName("backend_resolver_country")]
        public string? country_arecord { get; set; }
        /// <summary>
        /// AS-Number of the backend resolver IP address.
        /// </summary>
        [JsonPropertyName("backend_resolver_asn")]
        public int? asn_arecord { get; set; }
        /// <summary>
        /// Registered covering prefix of the backend resolver IP address.
        /// </summary>
        [JsonPropertyName("backend_resolver_prefix")]
        public string? prefix_arecord { get; set; }
        /// <summary>
        /// Organization name of the backend resolver ASN.
        /// </summary>
        [JsonPropertyName("backend_resolver_org")]
        public string? org_arecord { get; set; }
        /// <summary>
        /// Date of the latest scan in the database.
        /// </summary>
        public string? scan_date { get; set; }

    }
}
