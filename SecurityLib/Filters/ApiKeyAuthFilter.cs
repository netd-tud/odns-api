using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using SecurityEntities.Request;
using SecurityEntities.Response;
using SecurityRepository.SecurityMethods.Authorization.ApiKey;


namespace SecurityLib.Filters
{
    public class ApiKeyAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private IApiKeyRepository _apiKeyRepo;
        private ILogger<ApiKeyAuthFilter> _logger;

        public ApiKeyAuthFilter(IConfiguration configuration, ILogger<ApiKeyAuthFilter> logger, IApiKeyRepository apiKeyRepo) 
        {
            _configuration = configuration;
            _logger = logger;
            _apiKeyRepo = apiKeyRepo;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            StringValues apikey;
            if (!context.HttpContext.Request.Headers.TryGetValue(_configuration.GetSection("Security:HeaderTokenField").Value, out  apikey))
            {
                context.Result = new UnauthorizedObjectResult("Api key missing");
                return;
            }
            else 
            {
                try
                {
                    TokenInspectResponse response = await _apiKeyRepo.inspectToken(new TokenInspectRequest() { token = apikey[0] });
                    if (!response.valid)
                    {
                        context.Result = new UnauthorizedObjectResult("Wrong Api Key");
                        return;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error occured while checking Api Key Authorization {ex.ToString()}");
                    context.Result = new UnauthorizedObjectResult("Wrong Api Key");
                    return;
                }
            }
            //throw new NotImplementedException();
        }
    }
}
