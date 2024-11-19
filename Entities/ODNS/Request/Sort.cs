using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ODNS.Request
{
    public class Sort
    {
        public string field {  get; set; }
        public string order { get; set; } //"asc" | "desc"
    }
}
