using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics
{
    public interface IMetricsManager
    {
        void incrementRequestCounter(string route);
    }
}
