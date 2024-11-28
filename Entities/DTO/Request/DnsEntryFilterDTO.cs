using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.ODNS.Request;

namespace Entities.DTO.Request
{
    public class DnsEntryFilterDTO 
    {
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

        public DnsEntryFilterDTO(DnsEntryFilter parent)
        {
            this.protocol = parent.protocol;
            this.ip_request = parent.ip_request;
            this.ip_response = parent.ip_response;
            this.a_record = parent.a_record;
            this.timestamp_request = parent.timestamp_request;
            this.timestamp_response = parent.timestamp_response;
            this.response_type = parent.response_type;
            this.country_request = parent.country_request;
            this.country_response = parent.country_response;
            this.asn_request = parent.asn_request;
            this.asn_response = parent.asn_response;
            this.prefix_request = parent.prefix_request;
            this.prefix_response = parent.prefix_response;
            this.org_request = parent.org_request;
            this.org_response = parent.org_response;
            this.country_arecord = parent.country_arecord;
            this.asn_arecord = parent.asn_arecord;
            this.prefix_arecord = parent.prefix_arecord;
            this.org_arecord = parent.org_arecord;
    
        }
    
       
    }
}
