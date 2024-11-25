using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEntities.Response
{
    public class TokenInspectResponse
    {
        public bool valid {  get; set; }
        public string? expiry {  get; set; }
    }
}
