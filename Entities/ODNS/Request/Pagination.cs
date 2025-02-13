using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ODNS.Request
{
    public class Pagination
    {
        public int page {  get; set; } = 1;
        public int per_page { get; set; } = 10;
    }
}
