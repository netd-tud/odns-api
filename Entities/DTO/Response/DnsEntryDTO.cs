using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.ODNS.Request;
using Entities.ODNS.Response;

namespace Entities.DTO.Response
{
    public class DnsEntryDTO
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

        public DnsEntry GetDisplayResponse()
        {
            DnsEntry parent = new DnsEntry();

            
            parent.protocol = this.protocol;
            parent.ip_request = this.ip_request;
            parent.ip_response = this.ip_response;
            parent.a_record = this.a_record;
            parent.timestamp_request = this.timestamp_request;
            parent.timestamp_response = this.timestamp_response;
            parent.response_type = this.response_type;
            parent.country_request = this.country_request;
            parent.country_response = this.country_response;
            parent.asn_request = this.asn_request;
            parent.asn_response = this.asn_response;
            parent.prefix_request = this.prefix_request;
            parent.prefix_response = this.prefix_response;
            parent.org_request = this.org_request;
            parent.org_response = this.org_response;
            parent.country_arecord = this.country_arecord;
            parent.asn_arecord = this.asn_arecord;
            parent.prefix_arecord = this.prefix_arecord;
            parent.org_arecord = this.org_arecord;



            return parent;
        }
    }
}
