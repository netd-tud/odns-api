using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLib.SecurityMethods
{
    public interface ISecurityMethod
    {
        Task<string> requestToken(string tokenRequest);
        Task<string> revokeToken(string revokRequest);
        Task<string> inspect(string inspectRequest);

    }
}
