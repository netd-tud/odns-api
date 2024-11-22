using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityEntities.Request;
using SecurityEntities.Response;

namespace SecurityRepository.SecurityMethods.Authorization.ApiKey
{
    public interface IApiKeyRepository
    {
        Task<TokenRequestResponse> requestToken(TokenRequest tokenRequest);
    }
}
