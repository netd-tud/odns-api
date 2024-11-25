using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityEntities.Request;
using SecurityEntities.Response;

namespace SecurityLib.SecurityMethods
{
    public interface ISecurityMethod
    {
        Task<TokenRequestResponse> requestToken(TokenRequest request);
        Task<TokenInspectResponse> inspect(TokenInspectRequest request);
    }
}
