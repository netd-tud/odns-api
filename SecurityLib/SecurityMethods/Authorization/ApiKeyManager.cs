using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecurityEntities.Request;
using SecurityEntities.Response;
using SecurityRepository.SecurityMethods.Authorization.ApiKey;

namespace SecurityLib.SecurityMethods.Authorization
{
    public class ApiKeyManager 
    {
        private readonly ILogger<ApiKeyManager> _logger;
        private readonly IConfiguration _configuration;

        private IApiKeyRepository _apiKeyRepository;
        public ApiKeyManager(ILogger<ApiKeyManager> logger, IConfiguration configuration, IApiKeyRepositoryFactory apiKeyRepositoryFactory) 
        {
            _configuration = configuration;
            _logger = logger;

            string dbtype = _configuration["Database:dbtype"];
            _apiKeyRepository = apiKeyRepositoryFactory.GetInstance(dbtype);
        }

        public async Task<TokenRequestResponse> requestToken(TokenRequest tokenRequest)
        {
            try
            {
                // To-Do call database function to generate token and id
                // then return the id and send an email containing the id and the token


            }
            catch (Exception ex) 
            { 

            }
        }

        public void revokeToken()
        {

        }

        public void inspect()
        {

        }

    }
}
