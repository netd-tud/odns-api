using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecurityEntities.Request;
using SecurityEntities.Response;
using SecurityRepository.SecurityMethods.Authorization.ApiKey;

namespace SecurityLib.SecurityMethods.Authorization
{
    public class ApiKeyManager : ISecurityMethod
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

        public async Task<TokenRequestResponse> requestToken(TokenRequest request)
        {
            _logger.LogInformation($"{nameof(this.requestToken)} called rid: {request.rid} with the following request:\n {JsonSerializer.Serialize(request)}");
            TokenRequestResponse response = new TokenRequestResponse();
            try
            {
                // To-Do call database function to generate token and id
                // then return the id and send an email containing the id and the token
                response = await _apiKeyRepository.requestToken(request);
                // send email
                response.token=null;
                _logger.LogDebug($"GetDnsEntries response for rid: {request.rid}\n {JsonSerializer.Serialize(response)}");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error occured while executing {nameof(this.requestToken)}\n ex: {ex.ToString()}");
            }
            return response;
        }

        public void revokeToken()
        {

        }

        public async Task<TokenInspectResponse> inspect(TokenInspectRequest request)
        {
            _logger.LogInformation($"{nameof(this.inspect)} called rid: {request.rid} with the following request:\n {JsonSerializer.Serialize(request)}");
            TokenInspectResponse response = new TokenInspectResponse();
            try
            {
                // To-Do call database function to validate token
                response = await _apiKeyRepository.inspectToken(request);
                
                _logger.LogDebug($"GetDnsEntries response for rid: {request.rid}\n {JsonSerializer.Serialize(response)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while executing {nameof(this.inspect)}\n ex: {ex.ToString()}");
            }
            return response;
        }

    }
}
