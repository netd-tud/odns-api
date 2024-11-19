using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Enums
{
    public enum EProtocol
    {
        UDP=0, 
        TCP=1
    }

    public enum EResponseType
    {
        Resolver = 0,
        Forwarder = 1,
        TransparentForwarder = 2

    }
}
