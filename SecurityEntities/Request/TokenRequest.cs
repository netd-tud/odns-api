using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEntities.Request
{
    public class TokenRequest
    {
        public string affiliate_email {  get; set; }
        public string organization_name { get; set; }
        public string? personofcontact_name { get; set; }
        public string? personofcontact_phonenumber { get; set; } // To-Do add validation

    }
}
