using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace SecurityLib.Filters
{
    public class ApiKeyAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            
            if(!context.HttpContext.Request.Headers.TryGetValue("apikey",out  var apikey))
            {
                context.Result = new UnauthorizedObjectResult("Api key missing");
                return;
            }
            throw new NotImplementedException();
        }
    }
}
