using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ODNSRepository;
using ODNSRepository.Repository;
using Entities.Auth;

namespace AuthUtils
{

    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var services = context.HttpContext.RequestServices;
            ILogger<ApiKeyAuthAttribute> logger = services.GetRequiredService<ILogger<ApiKeyAuthAttribute>>();
            IConfiguration configuration = services.GetRequiredService<IConfiguration>();

            if (!request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result =  new ObjectResult("API Key is missing or invalid")
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Resolve your repository from the DI container
            IOdnsRepositoryFactory repoFac = services.GetRequiredService<IOdnsRepositoryFactory>();
            string dbtype = configuration["Database:dbtype"];
            IOdnsRepository dnsRepository = repoFac.GetInstance(dbtype);

            try
            {
                ApiKeyRecord apiKey = await dnsRepository.GetApiKeyRecord(new ApiKeyDTO { apikey = extractedApiKey });
                if (apiKey == default(ApiKeyRecord)) 
                {
                    context.Result = new ObjectResult("Unauthorized client.")
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }
            }
            catch (Exception ex) 
            {
                context.Result = new ObjectResult("Unauthorized client.")
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            await next();
        }
    }

}
