using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEntities.DTO
{
    public class TokenRequestReponseDTO
    {
        public string id { get; set; }
        public string? token { get; set; } // token is not needed always
    }
}
