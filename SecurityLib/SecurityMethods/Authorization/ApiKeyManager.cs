using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SecurityLib.SecurityMethods.Authorization
{
    public class ApiKeyManager
    {
        private readonly ILogger<ApiKeyManager> _logger;
        private readonly IConfiguration _configuration;
        public ApiKeyManager(ILogger<ApiKeyManager> logger, IConfiguration configuration) 
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void requestToken()
        {

        }

        public void revokeToken()
        {

        }

        public void inspect()
        {

        }

    }
}
