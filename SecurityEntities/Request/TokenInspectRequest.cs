using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEntities.Request
{
    public class TokenInspectRequest
    {
        public string? rid {  get; set; } = Guid.NewGuid().ToString();
        public string? id { get; set; }
        public string token {  get; set; }
    }
}
